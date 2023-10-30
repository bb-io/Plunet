using Blackbird.Applications.Sdk.Common;
using Blackbird.Plugins.Plunet.DataItem30Service;

namespace Blackbird.Plugins.Plunet.Models.Item;

public class PriceLineResponse
{
    [Display("Price line ID")]
    public string PriceLineId { get; set; }

    [Display("Price unit ID")]
    public string PriceUnitId { get; set; }

    [Display("Price amount")]
    public double PriceAmount { get; set; }

    [Display("Price amount per unit")]
    public double PriceAmountPerUnit { get; set; }

    [Display("Unit price")]
    public double UnitPrice { get; set; }

    [Display("Tax type")]
    public int TaxType { get; set; }

    [Display("Sequence")]
    public int Sequence { get; set; }

    public PriceLineResponse(PriceLine priceLine)
    {
        PriceAmount = priceLine.amount;
        PriceAmountPerUnit = priceLine.amount_perUnit;
        UnitPrice = priceLine.unit_price;
        PriceLineId = priceLine.priceLineID.ToString();
        PriceUnitId = priceLine.priceUnitID.ToString();
        Sequence = priceLine.sequence;
        TaxType = priceLine.taxType;
    }
}