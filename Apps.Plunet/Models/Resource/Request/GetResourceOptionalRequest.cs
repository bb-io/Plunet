using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Resource.Request;

public class GetResourceOptionalRequest
{
    [Display("Resource")]
    [DataSource(typeof(ResourceIdDataHandler))]
    public string? ResourceId { get; set; }
}