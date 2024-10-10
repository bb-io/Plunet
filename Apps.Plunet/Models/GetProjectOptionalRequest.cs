using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models;

public class GetProjectOptionalRequest
{
    [Display("Project ID")]
    public string? ProjectId { get; set; }
}