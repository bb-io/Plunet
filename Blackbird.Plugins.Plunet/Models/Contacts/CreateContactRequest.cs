namespace Blackbird.Plugins.Plunet.Models.Contacts;

public class CreateContactRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string CostCenter { get; set; }
}