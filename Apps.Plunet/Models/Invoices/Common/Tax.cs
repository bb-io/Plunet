using Newtonsoft.Json;

namespace Apps.Plunet.Models.Invoices.Common;

public class Tax
{
    [JsonProperty("description")]
    public string Description { get; set; }
    
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
}