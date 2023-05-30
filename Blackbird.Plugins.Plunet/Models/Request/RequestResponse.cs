namespace Blackbird.Plugins.Plunet.Models.Request;

public class RequestResponse
{
    public string BriefDescription { get; set; }
    public string Subject { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public int OrderId { get; set; }
    public DateTime QuotationDate { get; set; }
    public int QuoteId { get; set; }
    public int RequestId { get; set; }
    public int Status { get; set; }
}