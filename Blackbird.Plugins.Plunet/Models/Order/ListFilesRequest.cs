using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class ListFilesRequest
{
    [Display("Order id")] public int OrderId { get; set; }

    [Display("Folder type")] public int FolderType { get; set; }
}