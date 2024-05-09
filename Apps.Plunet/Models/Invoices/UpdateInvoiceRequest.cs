using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Models.Invoices;

public class UpdateInvoiceRequest : InvoiceRequest
{
    [Display("Invoice status"), StaticDataSource(typeof(InvoiceStatusDataHandler))]
    public string? InvoiceStatus { get; set; }

    public string? Subject { get; set; }
    
    [Display("Brief description")]
    public string? BriefDescription { get; set; }

    [Display("Invoice date")]
    public DateTime? InvoiceDate { get; set; }

    [Display("Paid date")]
    public DateTime? PaidDate { get; set; }
}