using Blackbird.Applications.Sdk.Common;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Plunet.Models.Document;

public class ListFilesResponse
{
    [Display("Files")]
    public IEnumerable<File> Files { get; set; }
}