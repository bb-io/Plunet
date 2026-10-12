using Blackbird.Applications.Sdk.Common;
using Blackbird.Plugins.Plunet.DataItem30Service;

namespace Apps.Plunet.Models;

public class PricelineResponse
{
    public PricelineResponse() { }
    
    public PricelineResponse(PriceLine line, PriceUnit? unit)
    {
        Amount = line.amount;
        AmountPerUnit = line.amount_perUnit;
        Memo = line.memo ?? string.Empty;
        Id = line.priceLineID.ToString();
        UnitPrice = line.unit_price;
        Sequence = line.sequence;
        TaxType = line.taxType.ToString() ?? string.Empty;
        TimePerUnit = line.time_perUnit;
        PriceUnitId = line.priceUnitID.ToString();
        PriceUnitDescription = unit?.description ?? "";
        PriceUnitService = unit?.service ?? "";
    }
    
    [Display("Priceline ID")]
    public string Id { get; set; }

    [Display("Amount")]
    public double Amount { get; set; }

    [Display("Amount per unit")]
    public double AmountPerUnit { get; set; }

    [Display("Memo")]
    public string Memo { get; set; }

    [Display("Sequence")]
    public int Sequence { get; set; }

    [Display("Tax type")]
    public string TaxType { get; set; }

    [Display("Time per unit")]
    public double TimePerUnit { get; set; }

    [Display("Unit price")]
    public double UnitPrice { get; set; }

    [Display("Price unit ID")]
    public string PriceUnitId { get; set; }

    [Display("Price unit description")]
    public string PriceUnitDescription { get; set; }

    [Display("Price unit service")]
    public string PriceUnitService { get; set; }
}