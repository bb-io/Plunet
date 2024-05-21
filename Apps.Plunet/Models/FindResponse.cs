using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models;

public class FindResponse<T>(T? item, int totalCount)
    where T : class
{
    public T? Item { get; set; } = item;

    [Display("Total count")]
    public int TotalCount { get; set; } = totalCount;
}