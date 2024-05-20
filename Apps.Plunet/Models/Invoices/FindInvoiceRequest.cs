using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Invoices;

public class FindInvoiceRequest : SearchInvoicesRequest
{
    [Display("Invoice number")]
    public string? InvoiceNumber { get; set; }
    
    [Display("Flag", Description = "e.g. [Textmodule_1]")]
    public string? Flag { get; set; }
    
    [Display("Text module value")]
    public string? TextModuleValue { get; set; }
}