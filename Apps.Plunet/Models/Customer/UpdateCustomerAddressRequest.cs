using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Customer;

public class UpdateCustomerAddressRequest
{
    [Display("Address ID")]
    public string AddressId { get; set; }
    
    [Display("Address type")]
    [StaticDataSource(typeof(AddressTypeDataHandler))]
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
    public string? Country { get; set; }
    
    public string? Description { get; set; }
}