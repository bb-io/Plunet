using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Resource.Request;

public class ResourceRequest
{
    [Display("Resource ID")]
    [DataSource(typeof(ResourceIdDataHandler))]
    public string ResourceId { get; set; }

    [Display("Resource contact ID")]
    public string? ResourceContactId { get; set; }
}
