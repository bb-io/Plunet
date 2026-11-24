using Blackbird.Plugins.Plunet.DataQuote30Service;

namespace Apps.Plunet.Extensions;

public static class QuoteExtensions
{
    public static string ToDisplayName(this Quote quote)
    {
        return string.IsNullOrWhiteSpace(quote.projectName) ? $"Unnamed quote (ID: {quote.quoteID})" : quote.projectName;
    }
}