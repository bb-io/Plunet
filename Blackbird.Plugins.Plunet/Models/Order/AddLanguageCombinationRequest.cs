using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class AddLanguageCombinationRequest
{
    [Display("Order ID")]
    public string OrderId { get; set; }

    [Display("Source language code")]
    public string SourceLanguageCode { get; set; }

    [Display("Target language code")]
    public string TargetLanguageCode { get; set; }
}