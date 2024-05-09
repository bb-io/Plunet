using Apps.Plunet.DataOutgoingInvoice30Service;
using Apps.Plunet.Models.Customer;
using Apps.Plunet.Models.Invoices.Items;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Invoices;

public class GetInvoiceResponse(InvoiceResult result, GetCustomerResponse? customer)
{
    [Display("Invoice ID")]
    public string InvoiceId { get; set; } = result.data.invoiceID.ToString();

    [Display("Project name")]
    public string ProjectName { get; set; } = result.data.briefDescription;

    [Display("Customer ID")]
    public string CustomerId { get; set; } = result.data.customerID.ToString();
    
    [Display("Customer")]
    public GetCustomerResponse? Customer { get; set; } = customer;

    [Display("Description")]
    public string Description { get; set; } = result.data.subject;

    [Display("Status")]
    public string Status { get; set; } = result.data.status.ToString();

    [Display("Currency code")]
    public string CurrencyCode { get; set; } = result.data.currencyCode;

    [Display("Gross")]
    public double Gross { get; set; } = result.data.gross;

    [Display("Invoice date")]
    public DateTime InvoiceDate { get; set; } = result.data.invoiceDate;

    [Display("Invoice number")]
    public string InvoiceNumber { get; set; } = result.data.invoiceNr;

    [Display("Net")]
    public double Net { get; set; } = result.data.net;

    [Display("Outgoing")]
    public double Outgoing { get; set; } = result.data.outgoing;

    [Display("Paid")]
    public double Paid { get; set; } = result.data.paid;

    [Display("Tax")]
    public double Tax { get; set; } = result.data.tax;

    [Display("Value date")]
    public DateTime ValueDate { get; set; } = result.data.valueDate;

    [Display("Invoice items")]
    public List<InvoiceItemResponse> InvoiceItems { get; set; }
}