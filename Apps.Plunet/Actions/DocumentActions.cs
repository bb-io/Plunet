using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Document;
using Apps.Plunet.Models.Enums;
using Apps.Plunet.Models.FFPicker;
using Apps.Plunet.Models.Item;
using Apps.Plunet.Models.Job;
using Apps.Plunet.Strategies;
using Apps.Plunet.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

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
            var pathInfo = ResolvePathInfo(request.FolderId, PickerMode.Folder);

            await ExecuteWithRetry(() => DocumentClient.upload_DocumentAsync(Uuid, pathInfo.MainId, pathInfo.FolderType,
                fileBytes, $"{pathInfo.Path?.Replace("/", "\\") ?? ""}\\{request.Subfolder?.Replace("/", "\\") ?? ""}\\{request.File.Name}",
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
        var pathInfo = ResolvePathInfo(request.FileId, PickerMode.File);

        return await DownloadFile(pathInfo.MainId, pathInfo.FolderType, $"{pathInfo.Path?.Replace("/", "\\") ?? ""}");
    }

    [Action("Download all files in folder", Description = "Download all the files from an entity folder")]
    public async Task<ListFilesResponse> ListFiles([ActionParameter] ListFilesRequest request)
    {
        var pathInfo = ResolvePathInfo(request.FolderId, PickerMode.Folder);

        var response = await ExecuteWithRetryAcceptNull(() => DocumentClient.getFileListAsync(Uuid, pathInfo.MainId, pathInfo.FolderType));
        if (response is null) 
            return new ListFilesResponse { Files = new List<FileReference>() };

        if (request.Subfolder is not null)
            response = response.Where(folder => folder.StartsWith($"\\{request.Subfolder.Replace("/", "\\").ToLower()}\\")).ToArray();

        var files = new List<FileReference>();
        foreach (var path in response)
        {
            if (request.Filters is not null && request.Filters.Any(path.Contains)) continue;

            var file = await DownloadFile(pathInfo.MainId, pathInfo.FolderType, path);
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
        var pathInfo = ResolvePathInfo(request.FolderId, PickerMode.Folder);

        var response = await ExecuteWithRetryAcceptNull(() => DocumentClient.getFileListAsync(Uuid, pathInfo.MainId, pathInfo.FolderType));

        if (response is null) 
            return new SearchFilesResponse { FilePaths = [] };

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
            if (request.Filters is not null && request.Filters.Any(path.Contains)) 
                continue;

            filePaths.Add(path);
        }

        return new SearchFilesResponse { FilePaths = filePaths };
    }
    
    [Action("Upload CAT report to item",
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

    [Action("Upload CAT report to job",
        Description = "Upload a report file into the report folder of the specified job.")]
    public async Task UploadCatReportJob([ActionParameter] GetJobRequest job,
        [ActionParameter] UploadCatReportToJobRequest input)
    {
        var fileBytes = fileManagementClient.DownloadAsync(input.File).Result.GetByteData().Result;
        var response = await ExecuteWithRetry(() => JobClient.setCatReport2Async(          
            Uuid, fileBytes,
            input.File.Name, fileBytes.Length,
            ParseId(input.CatType), ParseId(job.ProjectType),
            input.CopyResultsToJob, ParseId(job.JobId)));
        InvocationContext.Logger?.LogError(
                    $"[PlunetSetCATReport] StatusCode {response.Result.statusCode}, warning code {response.Result.warning_StatusCodeList}," +
                    $"status message {response.Result.statusMessage} ",
                    []);
    }

    private async Task<FileResponse> DownloadFile(int mainId, int folderType, string filePath)
    {
        var response = await ExecuteWithRetry(() => DocumentClient.download_DocumentAsync(Uuid, mainId, folderType, filePath));

        using var stream = new MemoryStream(response.fileContent);
        var file = await fileManagementClient.UploadAsync(stream, MimeTypes.GetMimeType(response.filename), Path.GetFileName(response.filename));
        return new(file);
    }

    private PathInfo ResolvePathInfo(string folderId, PickerMode mode)
    {
        var provider = new StrategyProvider(FfClientProvider, mode);
        var path = PathParser.Parse(folderId);

        var strategy = provider.GetStrategy(path);
        return strategy is null
            ? throw new PluginMisconfigurationException($"The path '{folderId}' is not supported.")
            : strategy.ResolvePathInfo(path);
    }
}