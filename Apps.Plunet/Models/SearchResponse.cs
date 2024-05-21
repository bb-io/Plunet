using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models;

public class SearchResponse<T>
{
    public List<T> Items { get; set; }

    [Display("Total count")] 
    public int TotalCount { get; set; }

    public SearchResponse()
    {
        Items = new List<T>();
        TotalCount = 0;
    }

    public SearchResponse(List<T> items)
    {
        Items = items;
        TotalCount = items.Count;
    }
}