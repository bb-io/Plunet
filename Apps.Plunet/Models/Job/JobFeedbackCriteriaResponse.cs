using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Job;

public class JobFeedbackCriteriaResponse
{
    [Display("Criteria")]
    public IEnumerable<JobFeedbackCriterionDto> Criteria { get; set; } = [];
}

public class JobFeedbackCriterionDto
{
    [Display("Criterion ID")]
    public string CriterionId { get; set; } = string.Empty;

    [Display("Name")]
    public string Name { get; set; } = string.Empty;

    [Display("Active")]
    public bool Active { get; set; }

    [Display("KO value")]
    public double KoValue { get; set; }

    [Display("Weighting")]
    public double Weighting { get; set; }

    [Display("Tooltip")]
    public string Tooltip { get; set; } = string.Empty;
}
