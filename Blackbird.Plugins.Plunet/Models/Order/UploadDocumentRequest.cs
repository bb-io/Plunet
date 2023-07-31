using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class UploadDocumentRequest
{
    [Display("Order ID")]
    public string OrderId { get; set; }

    [Display("Folder еype")]
    public string FolderType { get; set; }

    [Display("File сontent bytes")]
    public byte[] FileContentBytes { get; set; }

    [Display("File path")]
    public string FilePath { get; set; }
}