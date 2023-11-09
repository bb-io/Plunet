using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Resource.Response;

public class ListResourceResponse
{
    [Display("Resources")]
    public IEnumerable<ResourceResponse> Resources { get; set; }
}