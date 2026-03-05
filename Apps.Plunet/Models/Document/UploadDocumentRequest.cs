using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;


namespace Apps.Plunet.Models.Document;

public class UploadDocumentRequest
{
    [Display("Folder ID")]
    [FileDataSource(typeof(FolderPickerDataSourceHandler))]
    public string FolderId { get; set; }

    public FileReference File { get; set; }

    [Display("Subfolder")]
    public string? Subfolder { get; set; }

    [Display("Ignore if file already exists")]
    public bool? IgnoreIfFileAlreadyExists { get; set; }
}