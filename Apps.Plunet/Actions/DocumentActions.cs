using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.Plunet.Models.Document;
using System.Net.Mime;
using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Common.Files;
using Apps.Plunet.Models.Item;
using Blackbird.Plugins.Plunet.DataDocument30Service;
using Blackbird.Plugins.Plunet.DataItem30Service;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;
using Result = Blackbird.Plugins.Plunet.DataDocument30Service.Result;
using StringArrayResult = Blackbird.Plugins.Plunet.DataDocument30Service.StringArrayResult;

namespace Apps.Plunet.Actions;

[ActionList]
public class DocumentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : PlunetInvocable(invocationContext)
{
    [Action("Upload file", Description = "Upload a file to an entity")]
    public async Task UploadFile([ActionParameter] UploadDocumentRequest request)
    {
        var fileBytes = fileManagementClient.DownloadAsync(request.File).Result.GetByteData().Result;
        var response = await ExecuteWithRetry(async () =>
            await DocumentClient.upload_DocumentAsync(Uuid, ParseId(request.MainId), ParseId(request.FolderType),
                fileBytes, $"{request.Subfolder?.Replace("/", "\\") ?? ""}\\{request.File.Name}",
                fileBytes.Length));

        if (response.Result.statusMessage.Contains("already exists") &&
            request.IgnoreIfFileAlreadyExists.HasValue && request.IgnoreIfFileAlreadyExists.Value)
            return;

        if (response.Result.statusMessage != ApiResponses.Ok)
            throw new(response.Result.statusMessage);
    }

    [Action("Download file", Description = "Download a file from an entity")]
    public async Task<FileResponse> DownloadFile([ActionParameter] DownloadDocumentRequest request)
    {
        var response = await ExecuteWithRetry<FileResult>(async () => await DocumentClient.download_DocumentAsync(Uuid,
            ParseId(request.MainId),
            ParseId(request.FolderType), request.FilePathName.Replace("/", "\\")));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        using var stream = new MemoryStream(response.fileContent);
        var file = await fileManagementClient.UploadAsync(stream, MediaTypeNames.Application.Octet,
            Path.GetFileName(response.filename));
        return new(file);
    }

    [Action("Download all files in folder", Description = "Download all the files from an entity folder")]
    public async Task<ListFilesResponse> ListFiles([ActionParameter] ListFilesRequest request)
    {
        var response = await ExecuteWithRetry<StringArrayResult>(async () =>
            await DocumentClient.getFileListAsync(Uuid, ParseId(request.MainId), ParseId(request.FolderType)));

        if (response.data == null)
            return new ListFilesResponse { Files = new List<FileReference>() };

        if (request.Subfolder != null)
            response.data = response.data.Where(folder =>
                folder.StartsWith($"\\{request.Subfolder.Replace("/", "\\").ToLower()}\\")).ToArray();

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        var files = new List<FileReference>();
        foreach (var path in response.data)
        {
            if (request.Filters != null && request.Filters.Any(path.Contains)) continue;

            var file = await DownloadFile(new DownloadDocumentRequest
                { MainId = request.MainId, FolderType = request.FolderType, FilePathName = path });
            files.Add(file.File);
        }

        return new ListFilesResponse { Files = files };
    }

    [Action("Upload CAT report",
        Description = "Upload a report file into the report folder of the specified item.")]
    public async Task UploadCatReport([ActionParameter] GetItemRequest item,
        [ActionParameter] UploadCatReportRequest input)
    {
        var fileBytes = fileManagementClient.DownloadAsync(input.File).Result.GetByteData().Result;
        var response = await ExecuteWithRetry(async () => await ItemClient.setCatReport2Async(Uuid, fileBytes,
            input.File.Name, fileBytes.Length,
            input.OverwriteExistingPricelines, ParseId(input.CatType), ParseId(input.ProjectType),
            input.CopyResultsToItem, ParseId(item.ItemId)));

        if (response.Result.statusMessage != ApiResponses.Ok)
            throw new(response.Result.statusMessage);
    }

    [Action("Calculate word count",
        Description =
            "Calculate the word count of documents. Only text files such as .txt, .html, .xliff, .docx and .pdf are supported.")]
    public async Task<WordCountResponse> GetWordCount([ActionParameter] ListFilesRequest request)
    {
        var getFilesResponse = await ExecuteWithRetry<StringArrayResult>(async () =>
            await DocumentClient.getFileListAsync(Uuid, ParseId(request.MainId), ParseId(request.FolderType)));

        if (getFilesResponse.data == null)
        {
            return new();
        }

        if (request.Subfolder != null)
            getFilesResponse.data = getFilesResponse.data.Where(folder =>
                folder.StartsWith($"\\{request.Subfolder.Replace("/", "\\").ToLower()}\\")).ToArray();

        if (getFilesResponse.statusMessage != ApiResponses.Ok)
            throw new(getFilesResponse.statusMessage);

        var wordCount = 0;
        var wordCountItems = new List<WordCountItem>();
        foreach (var path in getFilesResponse.data)
        {
            if (request.Filters != null && request.Filters.Any(path.Contains)) continue;

            var fileContent = await DownloadFileContent(request.MainId, request.FolderType, path);
            var words = ProcessFileContent(path, fileContent);
            wordCount += words;
            wordCountItems.Add(new WordCountItem { DocumentName = path, WordCount = words });
        }

        return new WordCountResponse
        {
            TotalWordCount = wordCount,
            DocumentWordCountItems = wordCountItems
        };
    }

    private async Task<byte[]> DownloadFileContent(string mainId, string folderType, string path)
    {
        var response = await ExecuteWithRetry<FileResult>(async () => await DocumentClient.download_DocumentAsync(Uuid,
            ParseId(mainId),
            ParseId(folderType), path));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        return response.fileContent;
    }

    private int ProcessFileContent(string path, byte[] fileContent)
    {
        using var stream = new MemoryStream(fileContent);
        string content;

        if (path.EndsWith(".docx") || path.EndsWith(".doc"))
        {
            content = ReadDocxFile(stream);
        }
        else if (path.EndsWith(".pdf"))
        {
            content = ReadPdfFile(stream);
        }
        else
        {
            content = ReadTxtFile(stream).Result;
        }

        return CountWords(content);
    }

    private static string ReadPdfFile(Stream file)
    {
        var document = PdfDocument.Open(file);
        var text = string.Join(" ", document.GetPages().Select(x => x.Text));
        return text;
    }

    private static string ReadDocxFile(Stream file)
    {
        var document = WordprocessingDocument.Open(file, false);
        var text = document.MainDocumentPart?.Document?.Body?.InnerText ??
                   throw new("Failed to read document body of docx file.");
        return text;
    }

    private static async Task<string> ReadTxtFile(Stream file)
    {
        using var reader = new StreamReader(file);
        return await reader.ReadToEndAsync();
    }

    private static int CountWords(string text)
    {
        char[] punctuationCharacters = text.Where(char.IsPunctuation).Distinct().ToArray();
        var words = text.Split().Select(x => x.Trim(punctuationCharacters));
        return words.Count(x => !string.IsNullOrWhiteSpace(x));
    }

    private async Task<T> ExecuteWithRetry<T>(Func<Task<Result>> func, int maxRetries = 10, int delay = 1000)
        where T : Result
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();

            if (result.statusMessage == ApiResponses.Ok)
            {
                return (T)result;
            }

            if (result.statusMessage.Contains("session-UUID used is invalid"))
            {
                if (attempts < maxRetries)
                {
                    await Task.Delay(delay);
                    await RefreshAuthToken();
                    attempts++;
                    continue;
                }

                throw new($"No more retries left. Last error: {result.statusMessage}, Session UUID used is invalid.");
            }

            return (T)result;
        }
    }

    private async Task<upload_DocumentResponse> ExecuteWithRetry(Func<Task<upload_DocumentResponse>> func,
        int maxRetries = 10, int delay = 1000)
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();

            if (result.Result.statusMessage == ApiResponses.Ok)
            {
                return result;
            }

            if (result.Result.statusMessage.Contains("session-UUID used is invalid"))
            {
                if (attempts < maxRetries)
                {
                    await Task.Delay(delay);
                    await RefreshAuthToken();
                    attempts++;
                    continue;
                }

                throw new(
                    $"No more retries left. Last error: {result.Result.statusMessage}, Session UUID used is invalid.");
            }

            return result;
        }
    }

    private async Task<setCatReport2Response> ExecuteWithRetry(Func<Task<setCatReport2Response>> func,
        int maxRetries = 10, int delay = 1000)
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();

            if (result.Result.statusMessage == ApiResponses.Ok)
            {
                return result;
            }

            if (result.Result.statusMessage.Contains("session-UUID used is invalid"))
            {
                if (attempts < maxRetries)
                {
                    await Task.Delay(delay);
                    await RefreshAuthToken();
                    attempts++;
                    continue;
                }

                throw new(
                    $"No more retries left. Last error: {result.Result.statusMessage}, Session UUID used is invalid.");
            }

            return result;
        }
    }
}