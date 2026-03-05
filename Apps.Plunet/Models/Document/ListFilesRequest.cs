using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Plunet.Models.Document;

public class ListFilesRequest
{
    [Display("Folder ID")]
    [FileDataSource(typeof(FolderPickerDataSourceHandler))]
    public string FolderId { get; set; }

    [Display("Subfolder")]
    public string? Subfolder { get; set; }

    [Display("Ignore files", Description = "Ignores files that contain any of the following substrings. Can be used to f.e. filter file types like '.rtf' from the result.")]
    public IEnumerable<string>? Filters { get; set; }
}