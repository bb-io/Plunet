namespace Blackbird.Plugins.Plunet.Models.Item;

public class PriceLineResponse
{
    public int PriceLineId { get; set; }
    
    public int PriceUnitId { get; set; }
    
    public double PriceAmount { get; set; }
    
    public double PriceAmountPerUnit { get; set; }

    public double UnitPrice { get; set; }
    
    public int TaxType { get; set; }

    public int Sequence { get; set; }
}