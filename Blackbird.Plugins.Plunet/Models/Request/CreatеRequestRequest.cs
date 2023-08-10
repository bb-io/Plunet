using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Plugins.Plunet.DataSourceHandlers.EnumHandlers;

namespace Blackbird.Plugins.Plunet.Models.Request;

public class CreatеRequestRequest
{
    [Display("Subject")]
    public string? Subject { get; set; }

    [Display("Brief description")]
    public string? BriefDescription { get; set; }

    [Display("Status")]
    [DataSource(typeof(RequestStatusDataHandler))]
    public string? Status { get; set; } 
    
    [Display("Delivery date")]
    public DateTime? DeliveryDate { get; set; }

    [Display("Order ID")]
    public string? OrderId { get; set; }

    [Display("Quotation date")]
    public DateTime? QuotationDate { get; set; }

    [Display("Quote ID")]
    public string? QuoteId { get; set; }

}