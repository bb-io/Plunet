using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Contacts;

public class GetContactsResponse
{
    [Display("Contacts")]
    public IEnumerable<ContactObjectResponse> CustomerContacts { get; set; }
}