using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Plugins.Plunet.DataSourceHandlers;
using Blackbird.Plugins.Plunet.DataSourceHandlers.EnumHandlers;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class UploadDocumentRequest
{
    [Display("Order ID")]
    [DataSource(typeof(OrderIdDataHandler))]
    public string OrderId { get; set; }

    [Display("Folder type")]
    [DataSource(typeof(FolderTypeDataHandler))]
    public string FolderType { get; set; }

    public File File { get; set; }

    [Display("File path")]
    public string FilePath { get; set; }
}