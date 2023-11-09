using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Document;

public class LanguageCombinationsResponse
{
    [Display("Language combinations")]
    public IEnumerable<LanguageCombination> LanguageCombinations { get; set; }
}