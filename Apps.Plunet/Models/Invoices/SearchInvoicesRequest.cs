using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Invoices;

public class SearchInvoicesRequest : SearchInputBase
{
    [Display("Language code", Description = "If not provided, EN will be used.")]
    public string? LanguageCode { get; set; }

    [Display("Date from", Description = "If not provided, 2021-01-01T12:10:10 will be used.")]
    public DateTime? DateFrom { get; set; }
    
    [Display("Date to", Description = "If not provided, 2021-12-31T12:10:10 will be used.")]
    public DateTime? DateTo { get; set; }

    [Display("Customer ID"), DataSource(typeof(CustomerIdDataHandler))]
    public string? CustomerId { get; set; }

    [Display("Invoice status"), StaticDataSource(typeof(InvoiceStatusDataHandler))]
    public string? InvoiceStatus { get; set; }

    [Display("Only return IDs", Description = "If enabled, returns only IDs without fetching details for each item.")]
    public bool? OnlyReturnIds { get; set; }
}