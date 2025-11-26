using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Document;

public class SearchFilesResponse
{
    [Display("File paths")]
    public List<string> FilePaths { get; set; }
}
