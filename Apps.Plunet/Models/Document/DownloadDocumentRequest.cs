using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Document;

public class DownloadDocumentRequest
{
    [Display("Entity ID")]
    public string MainId { get; set; }

    [Display("Folder type")]
    [StaticDataSource(typeof(FolderTypeDataHandler))]
    public string FolderType { get; set; }

    [Display("File path")]
    public string FilePathName { get; set; }

}