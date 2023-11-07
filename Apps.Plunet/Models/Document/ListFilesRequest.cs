using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Plugins.Plunet.DataSourceHandlers;
using Blackbird.Plugins.Plunet.DataSourceHandlers.EnumHandlers;

namespace Apps.Plunet.Models.Document;

public class ListFilesRequest
{
    [Display("Entity ID")]
    public string MainId { get; set; }

    [Display("Folder type")]
    [DataSource(typeof(FolderTypeDataHandler))]
    public string FolderType { get; set; }

    [Display("Subfolder")]
    public string? Subfolder { get; set; }
}