using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models;

public class SearchInputBase
{
    [Display("Limit", Description = "Maximum number of results. By default, the limit is 50.")]
    public int? Limit { get; set; }
}