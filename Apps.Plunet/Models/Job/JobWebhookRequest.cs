using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Job;

public class JobWebhookRequest
{
    [Display("Return ID only", Description = "If enabled, only the job ID is returned without fetching full job details. Filter parameters that depend on job data (status, job type) will be ignored in this mode.")]
    public bool? ReturnIdOnly { get; set; }
}
