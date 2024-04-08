using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Request.Request;

public class SearchRequestsInput
{
    [Display("Source language")]
    [DataSource(typeof(LanguageIsoDataHandler))]
    public string? SourceLanguage { get; set; }

    [Display("Target language")]
    [DataSource(typeof(LanguageIsoDataHandler))]
    public string? TargetLanguage { get; set; }

    [Display("Request status")]
    [StaticDataSource(typeof(RequestStatusDataHandler))]
    public string? RequestStatus { get; set; }

    [Display("Date from")] public DateTime? DateFrom { get; set; }

    [Display("Date to")] public DateTime? DateTo { get; set; }

    [Display("Customer entry type")]
    [StaticDataSource(typeof(CustomerEntryTypeDataHandler))]
    public string? CustomerEntryType { get; set; }

    [Display("Customer ID")] public string? MainId { get; set; }
}