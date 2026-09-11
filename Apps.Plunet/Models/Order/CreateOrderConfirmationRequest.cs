using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Models.Order;

public class CreateOrderConfirmationRequest
{
    [Display("Template name")]
    public string TemplateName { get; set; } = string.Empty;
    
    [Display("Format ID"), StaticDataSource(typeof(FormatDataHandler))]
    public string FormatId { get; set; } = string.Empty;
}