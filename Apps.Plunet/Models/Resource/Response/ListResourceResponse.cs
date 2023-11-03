using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Resource.Response;

public class ListResourceResponse
{
    [Display("Resources")]
    public IEnumerable<ResourceResponse> Resources { get; set; }
}