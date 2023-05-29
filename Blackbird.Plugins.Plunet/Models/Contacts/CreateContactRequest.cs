namespace Blackbird.Plugins.Plunet.Models.Contacts;

public class CreateContactRequest
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string MobilePhone { get; set; }
}