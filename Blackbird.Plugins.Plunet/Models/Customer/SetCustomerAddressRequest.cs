using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Customer;

public class SetCustomerAddressRequest
{
    [Display("Customer ID")]
    public string CustomerId { get; set; }

    [Display("Address type")]
    public int? AddressType { get; set; } // 1 - Shipping address, 2 - Billing address, 3 - Other

    [Display("First address name")]
    public string? FirstAddressName { get; set; }

    [Display("Street")]
    public string? Street { get; set; }

    [Display("Street 2")]
    public string? Street2 { get; set; }

    [Display("ZIP code")]
    public string? ZipCode { get; set; }

    [Display("City")]
    public string? City { get; set; }

    [Display("State")]
    public string? State { get; set; }

    [Display("Country")]
    public string? Country { get; set; }
    
    public string? Description { get; set; }
}