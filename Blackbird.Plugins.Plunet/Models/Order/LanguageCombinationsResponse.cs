using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class LanguageCombinationsResponse
{
    [Display("Language combinations")]
    public IEnumerable<LanguageCombination> LanguageCombinations { get; set; }
}