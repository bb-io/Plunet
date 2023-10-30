using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Contacts;

public class GetContactExternalIdResponse
{
    [Display("Contact external ID")]
    public string ContactExternalId { get; set; }
}