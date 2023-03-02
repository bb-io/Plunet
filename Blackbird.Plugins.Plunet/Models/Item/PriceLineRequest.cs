namespace Blackbird.Plugins.Plunet.Models.Item;

public class PriceLineRequest
{
    public string UUID { get; set; }

    public int ItemId { get; set; }
    
    public double Amount { get; set; }

    public double UnitPrice { get; set; }
}