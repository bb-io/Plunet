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

    [Display("Ignore files", Description = "Ignores files that contain any of the following substrings. Can be used to f.e. filter file types like '.rtf' from the result.")]
    public IEnumerable<string>? Filters { get; set; }
}