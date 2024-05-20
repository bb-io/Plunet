using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Invoices;

public class InvoiceRequest
{
    [Display("Invoice ID"), DataSource(typeof(InvoiceDataHandler))]
    public string InvoiceId { get; set; }

    [Display("Get customer", Description = "If true, we will get Customer object. Default is false.")]
    public bool? GetCustomer { get; set; }

    [Display("Custom field keys", Description = "Custom field keys to get. Default is null.")]
    public IEnumerable<string>? CustomFieldKeys { get; set; }

    [Display("Custom field values", Description = "Custom field values to get. Default is null.")]
    public IEnumerable<string>? CustomFieldValues { get; set; }
}