using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Invoices;

public class SearchInvoicesRequest
{
    [Display("Language code", Description = "If not provided, EN will be used.")]
    public string? LanguageCode { get; set; }

    [Display("Date from", Description = "If not provided, 2021-01-01T12:10:10 will be used.")]
    public DateTime? DateFrom { get; set; }
    
    [Display("Date to", Description = "If not provided, 2021-12-31T12:10:10 will be used.")]
    public DateTime? DateTo { get; set; }

    [Display("Customer ID"), DataSource(typeof(CustomerIdDataHandler))]
    public string? CustomerId { get; set; }
}