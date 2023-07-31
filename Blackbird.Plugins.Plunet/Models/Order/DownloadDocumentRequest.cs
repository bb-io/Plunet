using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class DownloadDocumentRequest
{
    [Display("Order id")] public string OrderId { get; set; }

    [Display("Folder type")] public string FolderType { get; set; }

    [Display("File path name")] public string FilePathName { get; set; }

}