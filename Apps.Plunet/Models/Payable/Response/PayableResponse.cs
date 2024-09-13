using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Payable.Response;

public class PayableResponse
{
    [Display("Payable ID")]
    public string Id { get; set; }

    [Display("Status")]
    public string Status { get; set; }

    [Display("External invoice number")]    
    public string ExternalInvoiceNumber { get; set; }

    [Display("Company code")]
    public string CompanyCode { get; set; }

    [Display("Account statement")]
    public string AccountStatement { get; set; }

    [Display("Creditor account")]
    public string CreditorAccount { get; set; }

    [Display("Expense account")]
    public string ExpenseAccount { get; set; }

    [Display("Currency")]
    public string Currency { get; set; }

    [Display("Is exported")]
    public bool IsExported { get; set; }

    [Display("Memo")]
    public string Memo { get; set; }

    [Display("Resource ID")]
    public string ResourceId { get; set; }

    [Display("Creator ID")]
    public string CreatorResourceId { get; set; }

    [Display("Payment method")]
    public string PaymentMethod { get; set; }

    [Display("Total net amount")]
    public double TotalNetAmount { get; set; }

    //[Display("Total tax amount")]
    //public double TotalTaxAmount { get; set; }

    [Display("Invoice date")]
    public DateTime InvoiceDate { get; set; }

    [Display("Paid date")]
    public DateTime PaidDate { get; set; }

    [Display("Due date")]
    public DateTime DueDate { get; set; }

    [Display("Value date")]
    public DateTime ValueDate { get; set; }

    [Display("Items")]
    public IEnumerable<PayableItemResponse> Items { get; set; }

}