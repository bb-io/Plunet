using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.Plunet.Models.Document;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Common.Files;
using Apps.Plunet.Models.Item;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Plunet.Actions;

[ActionList("Documenst")]
public class DocumentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : PlunetInvocable(invocationContext)
{
    [Action("Upload file", Description = "Upload a file to an entity")]
    public async Task UploadFile([ActionParameter] UploadDocumentRequest request)
    {
        var fileBytes = fileManagementClient.DownloadAsync(request.File).Result.GetByteData().Result;
        try
        {
            await ExecuteWithRetry(() => DocumentClient.upload_DocumentAsync(Uuid, ParseId(request.MainId), ParseId(request.FolderType),
                fileBytes, $"{request.Subfolder?.Replace("/", "\\") ?? ""}\\{request.File.Name}",
                fileBytes.Length));
        } catch(PluginApplicationException ex)
        {
            if (ex.Message.Contains("already exists") && request.IgnoreIfFileAlreadyExists.HasValue && request.IgnoreIfFileAlreadyExists.Value)
            {
                return;
            }
            throw;
        }
    }

    [Action("Download file", Description = "Download a file from an entity")]
    public async Task<FileResponse> DownloadFile([ActionParameter] DownloadDocumentRequest request)
    {
        var response = await ExecuteWithRetry(() => DocumentClient.download_DocumentAsync(Uuid, ParseId(request.MainId), ParseId(request.FolderType), request.FilePathName.Replace("/", "\\")));

        using var stream = new MemoryStream(response.fileContent);
        var file = await fileManagementClient.UploadAsync(stream, MimeTypes.GetMimeType(response.filename), Path.GetFileName(response.filename));
        return new(file);
    }

    [Action("Download all files in folder", Description = "Download all the files from an entity folder")]
    public async Task<ListFilesResponse> ListFiles([ActionParameter] ListFilesRequest request)
    {
        var response = await ExecuteWithRetryAcceptNull(() => DocumentClient.getFileListAsync(Uuid, ParseId(request.MainId), ParseId(request.FolderType)));

        if (response == null)
            return new ListFilesResponse { Files = new List<FileReference>() };

        if (request.Subfolder != null)
            response = response.Where(folder =>
                folder.StartsWith($"\\{request.Subfolder.Replace("/", "\\").ToLower()}\\")).ToArray();

        var files = new List<FileReference>();
        foreach (var path in response)
        {
            if (request.Filters != null && request.Filters.Any(path.Contains)) continue;

            var file = await DownloadFile(new DownloadDocumentRequest { MainId = request.MainId, FolderType = request.FolderType, FilePathName = path });
            if (file.File.Name.Contains("\\"))
            {
                file.File.Name = file.File.Name.Split('\\').Last();
            }
            files.Add(file.File);
        }

        return new ListFilesResponse { Files = files };
    }

    [Action("Search files", Description = "Search for files in a folder and return file paths without downloading them")]
    public async Task<SearchFilesResponse> SearchFiles([ActionParameter] ListFilesRequest request)
    {
        var response = await ExecuteWithRetryAcceptNull(() =>
            DocumentClient.getFileListAsync(
                Uuid,
                ParseId(request.MainId),
                ParseId(request.FolderType))
        );

        if (response == null)
            return new SearchFilesResponse { FilePaths = new List<string>() };

        if (!string.IsNullOrWhiteSpace(request.Subfolder))
        {
            var normalized = request.Subfolder.Replace("/", "\\").ToLower();
            response = response
                .Where(path => path.ToLower().Contains(normalized))
                .ToArray();
        }

        var filePaths = new List<string>();
        foreach (var path in response)
        {
            if (request.Filters != null && request.Filters.Any(path.Contains))
                continue;

            filePaths.Add(path);
        }

        return new SearchFilesResponse { FilePaths = filePaths };
    }

    [Action("Upload CAT report",
        Description = "Upload a report file into the report folder of the specified item.")]
    public async Task UploadCatReport([ActionParameter] GetItemRequest item,
        [ActionParameter] UploadCatReportRequest input)
    {
        var fileBytes = fileManagementClient.DownloadAsync(input.File).Result.GetByteData().Result;
        var response = await ExecuteWithRetry(() => ItemClient.setCatReport2Async(Uuid, fileBytes,
            input.File.Name, fileBytes.Length,
            input.OverwriteExistingPricelines, ParseId(input.CatType), ParseId(input.ProjectType),
            input.CopyResultsToItem, ParseId(item.ItemId)));
        InvocationContext.Logger?.LogError(
                    $"[PlunetSetCATReport] StatusCode {response.Result.statusCode}, warning code {response.Result.warning_StatusCodeList}," +
                    $"status message {response.Result.statusMessage} ",
                    []);
    }      
}