namespace Apps.Plunet.Models.Quote.Response;

public record ListQuotesResponse(IEnumerable<QuoteResponse> Quotes);