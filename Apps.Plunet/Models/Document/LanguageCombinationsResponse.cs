using Blackbird.Applications.Sdk.Common;
using Blackbird.Plugins.Plunet.Models;

namespace Apps.Plunet.Models.Document;

public class LanguageCombinationsResponse
{
    [Display("Language combinations")]
    public IEnumerable<LanguageCombination> LanguageCombinations { get; set; }
}