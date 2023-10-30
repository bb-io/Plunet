using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Plugins.Plunet.DataSourceHandlers;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class AddLanguageCombinationRequest
{
    [Display("Order ID")]
    [DataSource(typeof(OrderIdDataHandler))]
    public string OrderId { get; set; }

    [Display("Source language")]
    [DataSource(typeof(LanguageDataHandler))]
    public string SourceLanguageCode { get; set; }

    [Display("Target language")]
    [DataSource(typeof(LanguageDataHandler))]
    public string TargetLanguageCode { get; set; }
}