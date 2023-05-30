namespace Blackbird.Plugins.Plunet.Models.Request;

public class CreatеRequestRequest
{
    public string Subject { get; set; }
    public string BriefDescription { get; set; }
    public int Status { get; set; } // 1 - IN_PREPARATION, 2 - PENDING, 5 - CANCELED, 6 - CHANGED_INTO_QUOTE, 7 - CHANGED_INTO_ORDER, 8 - NEW_AUTO, 9 - REJECTED
    public DateTime DeliveryDate { get; set; }
    public int OrderId { get; set; }
    public DateTime QuotationDate { get; set; }
    public int QuoteId { get; set; }

}