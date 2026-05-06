using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Job;

public class SendJobFeedbackRequest
{
    [Display("Job ID")]
    public string JobId { get; set; }
}
