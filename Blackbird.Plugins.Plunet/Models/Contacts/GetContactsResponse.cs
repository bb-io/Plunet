namespace Blackbird.Plugins.Plunet.Models.Contacts;

public class GetContactsResponse
{
    public IEnumerable<ContactObjectResponse> CustomerContacts { get; set; }
}