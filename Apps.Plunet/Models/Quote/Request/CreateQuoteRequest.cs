using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Quote.Request;

public class CreateQuoteRequest
{
    [Display("Currency")]
    public string? Currency { get; set; }

    [Display("Customer")]
    [DataSource(typeof(CustomerIdDataHandler))]
    public string? CustomerId { get; set; }

    [Display("Project manager Memo")]
    public string? ProjectManagerMemo { get; set; }

    [Display("Project name")]
    public string? ProjectName { get; set; }

    [Display("Reference number")]
    public string? ReferenceNumber { get; set; }

    [Display("Subject")]
    public string? Subject { get; set; }

    [Display("Status")]
    [StaticDataSource(typeof(QuoteStatusDataHandler))]
    public string? Status { get; set; }

    [Display("Contact")]
    [DataSource(typeof(ContactIdDataHandler))]
    public string? ContactId { get; set; }

    [Display("External ID")]
    public string? ExternalId { get; set; }

    [Display("Project manager")]
    [DataSource(typeof(ProjectManagerIdDataHandler))]
    public string? ProjectManagerId { get; set; }

    [Display("Project status")]
    [StaticDataSource(typeof(ProjectStatusDataHandler))]
    public string? ProjectStatus { get; set; }

    [Display("Request ID")]
    public string? RequestId { get; set; }

    [Display("Project category")]
    public string? ProjectCategory { get; set; }
}