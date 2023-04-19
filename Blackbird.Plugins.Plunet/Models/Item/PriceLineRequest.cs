namespace Blackbird.Plugins.Plunet.Models.Item;

public class PriceLineRequest
{
    public int ItemId { get; set; }
    
    public double Amount { get; set; }

    public double UnitPrice { get; set; }
}