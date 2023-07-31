using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Contacts;

public class GetContactsResponse
{
    [Display("Customer contacts")]
    public IEnumerable<ContactObjectResponse> CustomerContacts { get; set; }
}