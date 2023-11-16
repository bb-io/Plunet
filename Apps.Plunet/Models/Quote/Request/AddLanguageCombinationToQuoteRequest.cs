﻿namespace Apps.Plunet.Models.Quote.Request;

public class AddLanguageCombinationToQuoteRequest
{ 
    public int QuoteId { get; set; }

    public string SourceLanguageCode { get; set; }

    public string TargetLanguageCode { get; set; }
}