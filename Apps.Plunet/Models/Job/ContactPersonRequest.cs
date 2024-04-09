using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Job;

public class ContactPersonRequest
{
    [Display("Contact person ID")]
    [DataSource(typeof(ResourceIdDataHandler))]
    public string? ResourceId { get; set; }
}