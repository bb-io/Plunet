using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Item;

public class SetLanguageCombinationRequest
{
    [Display("Item ID")]
    public string ItemId { get; set; }

    [Display("Language combination ID")]
    public string LanguageCombinationId { get; set; }
}