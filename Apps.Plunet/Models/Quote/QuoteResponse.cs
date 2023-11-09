using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Quote;

public class QuoteResponse
{
    public string Currency { get; set; }

    [Display("Project name")] public string ProjectName { get; set; }

    public double Rate { get; set; }

    public QuoteResponse(Blackbird.Plugins.Plunet.DataQuote30Service.Quote quote)
    {
        Currency = quote.currency;
        ProjectName = quote.projectName;
        Rate = quote.rate;
    }
}