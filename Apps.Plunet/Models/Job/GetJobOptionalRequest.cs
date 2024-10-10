using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Job;

public class GetJobOptionalRequest
{
    [Display("Job ID")]
    public string? JobId { get; set; }
}