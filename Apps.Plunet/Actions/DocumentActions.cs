using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
using Apps.Plunet.Models.Document;
using File = Blackbird.Applications.Sdk.Common.Files.File;
using System.Net.Mime;
using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Order;

namespace Apps.Plunet.Actions
{
    [ActionList]
    public class DocumentActions : PlunetInvocable
    {
        public DocumentActions(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        [Action("Upload file", Description = "Upload a file to an entity")]
        public async Task UploadFile([ActionParameter] UploadDocumentRequest request)
        {
            var id = IntParser.Parse(request.MainId, nameof(request.MainId))!.Value;
            var folderType = IntParser.Parse(request.FolderType, nameof(request.FolderType))!.Value;


            await DocumentClient.upload_DocumentAsync(Uuid, id, folderType, request.File.Bytes, $"{request.Subfolder?.Replace("/", "\\") ?? ""}\\{request.File.Name}", request.File.Bytes.Length);
        }

        [Action("Download file", Description = "Download a file from an entity")]
        public async Task<FileResponse> DownloadFile([ActionParameter] DownloadDocumentRequest request)
        {
            var id = IntParser.Parse(request.MainId, nameof(request.MainId))!.Value;
            var folderType = IntParser.Parse(request.FolderType, nameof(request.FolderType))!.Value;

            var response = await DocumentClient.download_DocumentAsync(Uuid, id, folderType, request.FilePathName.Replace("/", "\\"));

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            return new(new(response.fileContent)
            {
                Name = Path.GetFileName(response.filename),
                ContentType = MediaTypeNames.Application.Octet
            });
        }

        [Action("Download all files in folder", Description = "Download all the files from an entity folder")]
        public async Task<ListFilesResponse> ListFiles([ActionParameter] ListFilesRequest request)
        {
            var id = IntParser.Parse(request.MainId, nameof(request.MainId))!.Value;
            var folderType = IntParser.Parse(request.FolderType, nameof(request.FolderType))!.Value;

            var response = await DocumentClient.getFileListAsync(Uuid, id, folderType);

            if (request.Subfolder != null)
                response.data = response.data.Where(folder => folder.StartsWith($"\\{request.Subfolder.Replace("/", "\\").ToLower()}\\")).ToArray();

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            List<File> files = new List<File>();

            foreach(var path in response.data)
            {
                var file = await DownloadFile(new DownloadDocumentRequest { MainId = request.MainId, FolderType = request.FolderType, FilePathName = path });
                files.Add(file.File);
            }

            return new ListFilesResponse { Files = files};

        }
    }
}
