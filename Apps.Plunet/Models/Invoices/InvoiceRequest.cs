using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Invoices;

public class InvoiceRequest
{
    [Display("Invoice ID"), DataSource(typeof(InvoiceDataHandler))]
    public string InvoiceId { get; set; }
}