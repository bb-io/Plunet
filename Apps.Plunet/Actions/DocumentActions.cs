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

namespace Apps.Plunet.Actions
{
    [ActionList]
    public class DocumentActions : PlunetInvocable
    {
        private readonly IFileManagementClient _fileManagementClient;
        public DocumentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(invocationContext)
        {
            _fileManagementClient = fileManagementClient;
        }

        [Action("Upload file", Description = "Upload a file to an entity")]
        public async Task UploadFile([ActionParameter] UploadDocumentRequest request)
        {
            var fileBytes = _fileManagementClient.DownloadAsync(request.File).Result.GetByteData().Result;
            await DocumentClient.upload_DocumentAsync(Uuid, ParseId(request.MainId), ParseId(request.FolderType), fileBytes, $"{request.Subfolder?.Replace("/", "\\") ?? ""}\\{request.File.Name}", fileBytes.Length);
        }

        [Action("Download file", Description = "Download a file from an entity")]
        public async Task<FileResponse> DownloadFile([ActionParameter] DownloadDocumentRequest request)
        {
            var response = await DocumentClient.download_DocumentAsync(Uuid, ParseId(request.MainId), ParseId(request.FolderType), request.FilePathName.Replace("/", "\\"));

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            using var stream = new MemoryStream(response.fileContent);
            var file = await _fileManagementClient.UploadAsync(stream, MediaTypeNames.Application.Octet, Path.GetFileName(response.filename));
            return new(file);
        }

        [Action("Download all files in folder", Description = "Download all the files from an entity folder")]
        public async Task<ListFilesResponse> ListFiles([ActionParameter] ListFilesRequest request)
        {
            var response = await DocumentClient.getFileListAsync(Uuid, ParseId(request.MainId), ParseId(request.FolderType));

            if (request.Subfolder != null)
                response.data = response.data.Where(folder => folder.StartsWith($"\\{request.Subfolder.Replace("/", "\\").ToLower()}\\")).ToArray();

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            List<FileReference> files = new List<FileReference>();

            foreach(var path in response.data)
            {
                var file = await DownloadFile(new DownloadDocumentRequest { MainId = request.MainId, FolderType = request.FolderType, FilePathName = path });
                files.Add(file.File);
            }

            return new ListFilesResponse { Files = files};

        }

        [Action("Upload CAT report", Description = "Upload a report file into the report folder of the specified item.")]
        public async Task UploadCatReport([ActionParameter] GetItemRequest item, [ActionParameter] UploadCatReportRequest input)
        {
            var fileBytes = _fileManagementClient.DownloadAsync(input.File).Result.GetByteData().Result;
            var response = await ItemClient.setCatReport2Async(Uuid, fileBytes, input.File.Name, fileBytes.Length,
                input.OverwriteExistingPricelines, ParseId(input.CatType), ParseId(input.ProjectType),
                input.CopyResultsToItem, ParseId(item.ItemId));

            if (response.Result.statusMessage != ApiResponses.Ok)
                throw new(response.Result.statusMessage);
        }
    }
}
