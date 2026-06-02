using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Quote.Request;

public class QuoteWebhookRequest
{
    [Display("Return ID only", Description = "If enabled, only the quote ID is returned without fetching full quote details. Filter parameters that depend on quote data (status, category) will be ignored in this mode.")]
    public bool? ReturnIdOnly { get; set; }
}
