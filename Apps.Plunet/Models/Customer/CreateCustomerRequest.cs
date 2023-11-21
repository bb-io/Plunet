using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Customer;

public class CreateCustomerRequest
{
    [Display("Name 1")] public string? Name1 { get; set; }

    [Display("Name 2")] public string? Name2 { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }

    [Display("Mobile phone")] public string? MobilePhone { get; set; }

    public int? Status { get; set; }

    [Display("Form of address")] public int? FormOfAddress { get; set; }

    [Display("Cost center")] public string? CostCenter { get; set; }

    [Display("Academic title")] public string? AcademicTitle { get; set; }

    [DataSource(typeof(CurrencyDataSourceHandler))]
    public string? Currency { get; set; }

    public string? Fax { get; set; }

    public string? Opening { get; set; }

    [Display("Full name")] public string? FullName { get; set; }

    [Display("External ID")] public string? ExternalId { get; set; }

    [Display("Skype ID")] public string? SkypeId { get; set; }

    [Display("User ID")] public string? UserId { get; set; }

    [Display("Address type")]
    [DataSource(typeof(AddressTypeDataHandler))]
    public new string? AddressType { get; set; }

    [Display("Country")]
    [DataSource(typeof(CountryDataSourceHandler))]
    public new string? Country { get; set; }

    [Display("First address name")] public string? FirstAddressName { get; set; }

    [Display("Street")] public string? Street { get; set; }

    [Display("Street 2")] public string? Street2 { get; set; }

    [Display("ZIP code")] public string? ZipCode { get; set; }

    [Display("City")] public string? City { get; set; }

    [Display("State")] public string? State { get; set; }

    public string? Description { get; set; }
}