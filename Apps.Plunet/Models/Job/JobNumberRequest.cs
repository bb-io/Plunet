using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Job;

public class JobNumberRequest
{
    [Display("Job number")]
    public string JobNumber { get; set; }
}
