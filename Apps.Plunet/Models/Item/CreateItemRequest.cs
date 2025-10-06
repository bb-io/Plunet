using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Models.Item;

public class CreateItemRequest
{    
    [Display("Brief description")]
    public string? BriefDescription { get; set; }

    [Display("Delivery date")]
    public DateTime? Deadline { get; set; }

    [Display("Comment")]
    public string? Comment { get; set; }

    [Display("Reference")]
    public string? Reference { get; set; }

    [Display("Status")]
    [StaticDataSource(typeof(ItemStatusDataHandler))]
    public string? Status { get; set; }

    [Display("Total price")]
    public double? TotalPrice { get; set; }

    [Display("Tax type")]
    [StaticDataSource(typeof(TaxTypeDataHandler))]
    public string? TaxType { get; set; }
}