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