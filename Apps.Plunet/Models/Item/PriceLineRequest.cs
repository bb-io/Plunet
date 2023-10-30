using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Item;

public class PriceLineRequest
{
    [Display("Item ID")]
    public string ItemId { get; set; }
    
    public double Amount { get; set; }

    [Display("Unit price")]
    public double UnitPrice { get; set; }
}