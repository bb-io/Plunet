using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Item;

public class SetLanguageCombinationRequest
{
    [Display("Item ID")]
    public string ItemId { get; set; }

    [Display("Language combination ID")]
    public string LanguageCombinationId { get; set; }
}