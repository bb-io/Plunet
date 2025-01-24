using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Resource.Request;

public class ResourceParameters
{
    [Display("Academic title")] 
    public string? AcademicTitle { get; set; }
    
    [Display("Cost center")]
    public string? CostCenter { get; set; }
    
    [Display("Currency"), DataSource(typeof(CurrencyDataSourceHandler))] 
    public string? Currency { get; set; }
    
    [Display("Email")] 
    public string? Email { get; set; }
    
    [Display("External ID")] 
    public string? ExternalId { get; set; }
    
    [Display("Fax")] 
    public string? Fax { get; set; }
    
    [Display("Form of address")] 
    public string? FormOfAddress { get; set; }
    
    [Display("Full name")] 
    public string? FullName { get; set; }
    
    [Display("Mobile phone")] 
    public string? MobilePhone { get; set; }
    
    [Display("Last name")] 
    public string? Name1 { get; set; }
    
    [Display("First name")] 
    public string? Name2 { get; set; }
    
    [Display("Opening")] 
    public string? Opening { get; set; }
    
    [Display("Phone")] 
    public string? Phone { get; set; }
    
    [Display("Skype")]
    public string? SkypeId { get; set; }
    
    [Display("Website")] 
    public string? Website { get; set; }
    
    [Display("Status"), StaticDataSource(typeof(ResourceStatusDataHandler))] 
    public string? Status { get; set; }
    
    [Display("Supervisor 1")] 
    public string? Supervisor1 { get; set; }
    
    [Display("Supervisor 2")] 
    public string? Supervisor2 { get; set; }
    
    [Display("User ID")] 
    public string? UserId { get; set; }
    
    [Display("Working status"), StaticDataSource(typeof(WorkingStatusDataHandler))] 
    public string? WorkingStatus { get; set; }

    [Display("Delivery address name 1")]
    public string? DeliveryName1 { get; set; }

    [Display("Delivery address name 2")]
    public string? DeliveryName2 { get; set; }

    [Display("Delivery address description")]
    public string? DeliveryDescription { get; set; }

    [DataSource(typeof(CountryDataSourceHandler))]
    [Display("Delivery address country")]
    public string? DeliveryCountry { get; set; }

    [Display("Delivery address state")]
    public string? DeliveryState { get; set; }

    [Display("Delivery address city")]
    public string? DeliveryCity { get; set; }

    [Display("Delivery address street")]
    public string? DeliveryStreet { get; set; }

    [Display("Delivery address street 2")]
    public string? DeliveryStreet2 { get; set; }

    [Display("Delivery address ZIP code")]
    public string? DeliveryZipCode { get; set; }

    [Display("Delivery address office")]
    public string? DeliveryOffice { get; set; }

    [Display("Invoice address name 1")]
    public string? InvoiceName1 { get; set; }

    [Display("Invoice address name 2")]
    public string? InvoiceName2 { get; set; }

    [Display("Invoice address description")]
    public string? InvoiceDescription { get; set; }

    [DataSource(typeof(CountryDataSourceHandler))]
    [Display("Invoice address country")]
    public string? InvoiceCountry { get; set; }

    [Display("Invoice address state")]
    public string? InvoiceState { get; set; }

    [Display("Invoice address city")]
    public string? InvoiceCity { get; set; }

    [Display("Invoice address street")]
    public string? InvoiceStreet { get; set; }

    [Display("Invoice address street 2")]
    public string? InvoiceStreet2 { get; set; }

    [Display("Invoice address ZIP code")]
    public string? InvoiceZipCode { get; set; }

    [Display("Invoice address office")]
    public string? InvoiceOffice { get; set; }

    [Display("Account holder")]
    public string? AccountHolder { get; set; }

    [Display("Account ID")]
    public string? AccountId { get; set; }

    [Display("SWIFT - BIC")]
    public string? Bic { get; set; }

    [Display("Contract number")]
    public string? ContractNumber { get; set; }

    [Display("Debit account")]
    public string? DebitAccount { get; set; }

    [Display("IBAN")]
    public string? Iban { get; set; }

    [Display("Payment method ID")]
    public string? PaymentMethodId { get; set; }

    [Display("Preselected tax ID")]
    public string? PreselectdTaxId { get; set; }

    [Display("Sales tax ID")]
    public string? SalesTaxId { get; set; }
}