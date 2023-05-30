namespace Blackbird.Plugins.Plunet.Models.Customer;

public class UpdateCustomerRequest
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string MobilePhone { get; set; }
    public string Telephone { get; set; }
    public string Website { get; set; }
}