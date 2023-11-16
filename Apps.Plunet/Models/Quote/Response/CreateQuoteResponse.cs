using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Quote.Response;

public class CreateQuoteResponse
{
    [Display("Quote ID")]
    public string QuoteId { get; set; }
}