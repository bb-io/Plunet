using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Plugins.Plunet.DataSourceHandlers;
using Blackbird.Plugins.Plunet.DataSourceHandlers.EnumHandlers;

namespace Blackbird.Plugins.Plunet.Models.Customer;

public class SetCustomerAddressRequest
{

    [Display("Address type")]
    [DataSource(typeof(AddressTypeDataHandler))]
    public string AddressType { get; set; }

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
    [DataSource(typeof(CountryDataSourceHandler))]
    public string Country { get; set; }
    
    public string? Description { get; set; }
    
    public SetCustomerAddressRequest(CreateCustomerRequest createCustomer)
    {
        AddressType = createCustomer.AddressType ?? string.Empty;
        FirstAddressName = createCustomer.FirstAddressName;
        Street = createCustomer.Street;
        Street2 = createCustomer.Street2;
        ZipCode = createCustomer.ZipCode;
        City = createCustomer.City;
        State = createCustomer.State;
        Country = createCustomer.Country ?? string.Empty;
        Description = createCustomer.Description;
    }    
    
    public SetCustomerAddressRequest()
    {

    }
}