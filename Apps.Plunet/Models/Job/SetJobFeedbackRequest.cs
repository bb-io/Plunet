using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Job;

public class SetJobFeedbackRequest
{
    [Display("Action for unfulfilled KO criterion")]
    public string? ActionForUnfulfilledKoCriterion { get; set; }

    public string? Commentary { get; set; }

    [Display("External editor user ID")]
    public string? ExternalEditorUserId { get; set; }

    [Display("Is job quality rating closed")]
    public bool? IsJobQualityRatingClosed { get; set; }

    [Display("Criterion IDs")]
    public IEnumerable<string>? CriterionIds { get; set; }

    [Display("Critical amounts")]
    public IEnumerable<double>? CriticalAmounts { get; set; }

    [Display("Hard amounts")]
    public IEnumerable<double>? HardAmounts { get; set; }

    [Display("Minor amounts")]
    public IEnumerable<double>? MinorAmounts { get; set; }

    [Display("Criterion ratings")]
    public IEnumerable<int>? Ratings { get; set; }
}
