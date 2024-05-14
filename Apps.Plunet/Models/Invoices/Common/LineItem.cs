using Newtonsoft.Json;

namespace Apps.Plunet.Models.Invoices.Common;

public class LineItem
{
    [JsonProperty("description")]
    public string Description { get; set; }
    
    [JsonProperty("quantity")]
    public int Quantity { get; set; }
    
    [JsonProperty("unit_price")]
    public decimal UnitPrice { get; set; }
    
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
}