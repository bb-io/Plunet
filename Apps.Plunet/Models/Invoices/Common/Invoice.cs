using Newtonsoft.Json;

namespace Apps.Plunet.Models.Invoices.Common;

public class Invoice
{
    [JsonProperty("customer_name")]
    public string CustomerName { get; set; }
    
    [JsonProperty("invoice_number")]
    public string InvoiceNumber { get; set; }
    
    [JsonProperty("invoice_date")]
    public DateTime InvoiceDate { get; set; }
    
    [JsonProperty("currency")]
    public string Currency { get; set; }
    
    [JsonProperty("lines")]
    public List<LineItem> Lines { get; set; }
    
    [JsonProperty("sub_total")]
    public decimal SubTotal { get; set; }
    
    [JsonProperty("taxes")]
    public List<Tax> Taxes { get; set; }
    
    [JsonProperty("total")]
    public decimal Total { get; set; }
    
    [JsonProperty("custom_fields")]
    public Dictionary<string, string> CustomFields { get; set; }
}
