using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Item;

public class PricelistUnitPriceResponse
{
    [Display("Unit price")]
    public double UnitPrice { get; set; }

    [Display("Amount per unit")]
    public double AmountPerUnit { get; set; }

    [Display("Price unit ID")]
    public string PriceUnitId { get; set; }

    [Display("Price unit description")]
    public string PriceUnitDescription { get; set; }

    [Display("Price unit service")]
    public string PriceUnitService { get; set; }

    [Display("Price list ID")]
    public string PricelistId { get; set; }

    [Display("Priceline ID")]
    public string PricelineId { get; set; }
}
