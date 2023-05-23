namespace Blackbird.Plugins.Plunet.Models.Contacts;

public class UpdateContactRequest
{
    public int ContactId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string CostCenter { get; set; }
}