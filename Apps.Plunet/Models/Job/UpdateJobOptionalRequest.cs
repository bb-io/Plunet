using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Job;

public class UpdateJobOptionalRequest
{
    [Display("Comment", Description = "Work instruction comment of the job")]
    public string? Comment { get; set; }
}
