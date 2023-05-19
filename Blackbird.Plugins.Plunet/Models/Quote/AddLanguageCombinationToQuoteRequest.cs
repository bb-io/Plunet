namespace Blackbird.Plugins.Plunet.Models.Order;

public class AddLanguageCombinationToQuoteRequest
{ 
    public int QuoteId { get; set; }

    public string SourceLanguageCode { get; set; }

    public string TargetLanguageCode { get; set; }
}