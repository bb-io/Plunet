using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Quote.Request;

public class SearchQuotesInput : SearchInputBase
{
    [Display("Source language")]
    [DataSource(typeof(LanguageIsoDataHandler))]
    public string? SourceLanguage { get; set; }

    [Display("Target language")]
    [DataSource(typeof(LanguageIsoDataHandler))]
    public string? TargetLanguage { get; set; }
    
    [Display("Date from")] public DateTime? DateFrom { get; set; }

    [Display("Date to")] public DateTime? DateTo { get; set; }
    
    [Display("Customer entry type")]
    [StaticDataSource(typeof(CustomerEntryTypeDataHandler))]
    public string? CustomerEntryType { get; set; }

    [Display("Customer ID")] public string? CustomerMainId { get; set; }
    
    [Display("Quote status")]
    [StaticDataSource(typeof(QuoteStatusDataHandler))]
    public string? QuoteStatus { get; set; }
    
    [Display("Resource entry type")]
    public string? ResourceEntryType { get; set; }

    [Display("Resource ID")] public string? ResourceMainId { get; set; }

    [Display("Only return IDs", Description = "If enabled, returns only IDs without fetching details for each item.")]
    public bool? OnlyReturnIds { get; set; }
}