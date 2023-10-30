using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Quote;

public class QuoteResponse
{
    public string Currency { get; set; }

    [Display("Project name")] public string ProjectName { get; set; }

    public double Rate { get; set; }

    public QuoteResponse(DataQuote30Service.Quote quote)
    {
        Currency = quote.currency;
        ProjectName = quote.projectName;
        Rate = quote.rate;
    }
}