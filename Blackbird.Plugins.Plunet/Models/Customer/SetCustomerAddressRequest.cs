namespace Blackbird.Plugins.Plunet.Models.Customer;

public class SetCustomerAddressRequest
{
    public int CustomerId { get; set; }
    public int AddressType { get; set; } // 1 - Shipping address, 2 - Billing address, 3 - Other
    public string FirstAddressName { get; set; }
    public string SecondAddressName { get; set; }
    public string Street { get; set; }
    public string Street2 { get; set; }
    public string ZIPCode { get; set; }
    public string City { get; set; }
    public string State { get; set; }
}