using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Quote;

public class UpdateQuoteRequest : CreateQuoteRequest
{
    [Display("Quote ID")] public string QuoteId { get; set; }
}