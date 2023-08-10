using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Plugins.Plunet.DataSourceHandlers.EnumHandlers;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class ListFilesRequest
{
    [Display("Order id")] public string OrderId { get; set; }

    [Display("Folder type")] 
    [DataSource(typeof(FolderTypeDataHandler))]
    public string FolderType { get; set; }
}