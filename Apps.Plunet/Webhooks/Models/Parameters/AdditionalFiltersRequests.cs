using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Webhooks.Models.Parameters;

public class AdditionalFiltersRequests
{
    [Display("Description contains")]
    public string? DescriptionContains { get; set; }
}