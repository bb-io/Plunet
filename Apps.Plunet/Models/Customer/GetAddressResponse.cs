using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Customer;

    public class GetAddressResponse
    {
    [DefinitionIgnore]
    public string AddressID { get; set; }

    [Display("Address type")]
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
    public string Country { get; set; }

    public string? Description { get; set; }

    [Display("Sales tax type")]
    public string? SalesTaxType { get; set; }

    [Display("Cost center")]
    public string? CostCenter { get; set; }
}

