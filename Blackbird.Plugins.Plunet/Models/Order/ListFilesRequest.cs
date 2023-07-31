using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class ListFilesRequest
{
    [Display("Order id")] public string OrderId { get; set; }

    [Display("Folder type")] public string FolderType { get; set; }
}