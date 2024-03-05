using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Quote.Response;

public class QuoteResponse
{
    [Display("Quote ID")] public string QuoteId { get; set; }

    [Display("Customer ID")]
    public string? CustomerId { get; set; }

    [Display("Contact ID")]
    public string? ContactId { get; set; }

    [Display("Order ID")] public string? OrderId { get; set; }

    public string Currency { get; set; }

    [Display("Project name")] public string ProjectName { get; set; }

    public double Rate { get; set; }

    public string Status { get; set; }

    public string Subject { get; set; }

    public string Number { get; set; }

    [Display("Creation date")] public DateTime CreationDate { get; set; }

    [Display("Is creation date specified")]
    public bool CreationDateSpecificied { get; set; }

    [Display("Total price")] public double TotalPrice { get; set; }
    [Display("Project manager ID")]
    public string? ProjectManagerId { get; set; }

    public QuoteResponse(Blackbird.Plugins.Plunet.DataQuote30Service.Quote quote)
    {
        Currency = quote.currency;
        ProjectName = quote.projectName;
        Rate = quote.rate;
        Status = quote.status.ToString();
        Subject = quote.subject;
        Number = quote.quoteNumber;
        CreationDate = quote.creationDate;
        CreationDateSpecificied = quote.creationDateSpecified;
        QuoteId = quote.quoteID.ToString();
    }
}