namespace Apps.Plunet.Models.Invoices;

public class FindInvoiceRequest : SearchInvoicesRequest
{
    public string? InvoiceNumber { get; set; }
}