using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Models.Item;

public class ItemPricelineRequest
{
    [Display("Amount per unit")]
    public double? AmountPerUnit { get; set; }

    [Display("Total amount")]
    public double Amount { get; set; }

    [Display("Memo")]
    public string? Memo { get; set; }

    [Display("Time per unit")]
    public double? TimePerUnit { get; set; }

    [Display("Unit price")]
    public double? UnitPrice { get; set; }

    [Display("Tax type")]
    [StaticDataSource(typeof(TaxTypeDataHandler))]
    public string? TaxType { get; set; }
}
