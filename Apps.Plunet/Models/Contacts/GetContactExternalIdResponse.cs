using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Contacts;

public class GetContactExternalIdResponse
{
    [Display("Contact external ID")]
    public string ContactExternalId { get; set; }
}