using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Request;

public class CreatеRequestRequest
{
    [Display("Subject")]
    public string? Subject { get; set; }

    [Display("Brief description")]
    public string? BriefDescription { get; set; }

    [Display("Status")]
    public int? Status { get; set; } // 1 - IN_PREPARATION, 2 - PENDING, 5 - CANCELED, 6 - CHANGED_INTO_QUOTE, 7 - CHANGED_INTO_ORDER, 8 - NEW_AUTO, 9 - REJECTED

    [Display("Delivery date")]
    public DateTime? DeliveryDate { get; set; }

    [Display("Order ID")]
    public string? OrderId { get; set; }

    [Display("Quotation date")]
    public DateTime? QuotationDate { get; set; }

    [Display("Quote ID")]
    public string? QuoteId { get; set; }

}