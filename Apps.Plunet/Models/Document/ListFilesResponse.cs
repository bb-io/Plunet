using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;


namespace Apps.Plunet.Models.Document;

public class ListFilesResponse
{
    [Display("Files")]
    public IEnumerable<FileReference> Files { get; set; }
}