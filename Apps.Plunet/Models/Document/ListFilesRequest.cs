using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Document;

public class ListFilesRequest
{
    [Display("Entity ID")]
    public string MainId { get; set; }

    [Display("Folder type")]
    [StaticDataSource(typeof(FolderTypeDataHandler))]
    public string FolderType { get; set; }

    [Display("Subfolder")]
    public string? Subfolder { get; set; }
}