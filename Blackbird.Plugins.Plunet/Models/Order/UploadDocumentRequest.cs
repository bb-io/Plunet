using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Plugins.Plunet.DataSourceHandlers.EnumHandlers;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class UploadDocumentRequest
{
    [Display("Order ID")]
    public string OrderId { get; set; }

    [Display("Folder type")]
    [DataSource(typeof(FolderTypeDataHandler))]
    public string FolderType { get; set; }

    [Display("File сontent bytes")]
    public byte[] FileContentBytes { get; set; }

    [Display("File path")]
    public string FilePath { get; set; }
}