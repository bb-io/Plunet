using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Invoices.Items;

public class InvoiceItemResponse
{
    [Display("Invoice ID")]
    public string InvoiceId { get; set; }
    
    [Display("Invoice item ID")]
    public string InvoiceItemId { get; set; }
    
    [Display("Item number")]
    public string ItemNumber { get; set; }
    
    [Display("Language combination ID")]
    public string LanguageCombinationId { get; set; }
    
    [Display("Order ID")]
    public string OrderId { get; set; }
    
    [Display("Order item ID")]
    public string OrderItemId { get; set; }
    
    [Display("Total price")]
    public double TotalPrice { get; set; }
    
    [Display("Brief description")]
    public string BriefDescription { get; set; }
    
    [Display("Comment")]
    public string Comment { get; set; }
}