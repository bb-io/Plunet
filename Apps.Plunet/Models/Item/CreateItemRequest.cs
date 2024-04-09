using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

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
}