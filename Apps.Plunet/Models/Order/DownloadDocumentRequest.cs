using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Plugins.Plunet.DataSourceHandlers;
using Blackbird.Plugins.Plunet.DataSourceHandlers.EnumHandlers;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class DownloadDocumentRequest
{
    [Display("Order ID")]
    [DataSource(typeof(OrderIdDataHandler))]
    public string OrderId { get; set; }

    [Display("Folder type")] 
    [DataSource(typeof(FolderTypeDataHandler))]
    public string FolderType { get; set; }

    [Display("File path name")] public string FilePathName { get; set; }

}