using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Item;

public class GetItemOptionalRequest
{
    [Display("Item ID")]
    public string? ItemId { get; set; }
}