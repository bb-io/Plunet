using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Order;

public class AddLanguageCombinationRequest
{
    [Display("Order ID")]
    [DataSource(typeof(OrderIdDataHandler))]
    public string OrderId { get; set; }

    [Display("Source language")]
    [DataSource(typeof(LanguageIsoDataHandler))]
    public string SourceLanguageCode { get; set; }

    [Display("Target language")]
    [DataSource(typeof(LanguageIsoDataHandler))]
    public string TargetLanguageCode { get; set; }
}