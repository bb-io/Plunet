using Apps.Plunet.DataOutgoingInvoice30Service;
using Apps.Plunet.Models.Customer;
using Apps.Plunet.Models.Invoices.Items;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Invoices;

public class GetInvoiceResponse(Invoice result, GetCustomerResponse? customer)
{
    [Display("Invoice ID")]
    public string InvoiceId { get; set; } = result.invoiceID.ToString();

    [Display("Project name")]
    public string ProjectName { get; set; } = result.briefDescription;

    [Display("Customer ID")]
    public string CustomerId { get; set; } = result.customerID.ToString();
    
    [Display("Customer")]
    public GetCustomerResponse? Customer { get; set; } = customer;

    [Display("Description")]
    public string Description { get; set; } = result.subject;

    [Display("Status")]
    public string Status { get; set; } = result.status.ToString();

    [Display("Currency code")]
    public string CurrencyCode { get; set; } = result.currencyCode;

    [Display("Gross")]
    public double Gross { get; set; } = result.gross;

    [Display("Invoice date")]
    public DateTime InvoiceDate { get; set; } = result.invoiceDate;

    [Display("Invoice number")]
    public string InvoiceNumber { get; set; } = result.invoiceNr;

    [Display("Net")]
    public double Net { get; set; } = result.net;

    [Display("Outgoing")]
    public double Outgoing { get; set; } = result.outgoing;

    [Display("Paid")]
    public double Paid { get; set; } = result.paid;

    [Display("Tax")]
    public double Tax { get; set; } = result.tax;

    [Display("Value date")]
    public DateTime ValueDate { get; set; } = result.valueDate;

    [Display("Invoice items")]
    public List<InvoiceItemResponse> InvoiceItems { get; set; }
}