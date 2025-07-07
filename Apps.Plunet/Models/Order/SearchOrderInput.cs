using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Order;

public class SearchOrderInput : SearchInputBase
{
    [Display("Date from")] public DateTime? DateFrom { get; set; }

    [Display("Date to")] public DateTime? DateTo { get; set; }

    [Display("Customer")]
    [DataSource(typeof(CustomerIdDataHandler))]
    public string? CustomerId { get; set; }

    [Display("Source language")]
    [DataSource(typeof(LanguageIsoDataHandler))]
    public string? SourceLanguage { get; set; }

    [Display("Target language")]
    [DataSource(typeof(LanguageIsoDataHandler))]
    public string? TargetLanguage { get; set; }

    [Display("Status")]
    [StaticDataSource(typeof(ProjectStatusDataHandler))]
    public string? OrderStatus { get; set; }

    [Display("Project name")]
    public string? ProjectName { get; set; }

    [Display("Project description")]
    public string? ProjectDescription { get; set; }

    [Display("Project type")]
    [StaticDataSource(typeof(ProjectTypeDataHandler))]
    public string? ProjectType { get; set; }

    [Display("Item status")]
    [StaticDataSource(typeof(ItemStatusDataHandler))]
    public IEnumerable<string>? ItemStatus { get; set; }
}