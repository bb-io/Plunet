using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Quote.Response;

public class QuoteIdResponse
{
    [Display("Quote ID")]
    public string QuoteId { get; set; } = string.Empty;
}