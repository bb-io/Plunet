using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Request.Response;

public class RequestResponse
{
    [Display("Brief description")] public string BriefDescription { get; set; }

    [Display("Subject")] public string Subject { get; set; }

    [Display("Creation date")] public DateTime CreationDate { get; set; }

    [Display("Delivery date")] public DateTime DeliveryDate { get; set; }

    [Display("Order ID")] public string OrderId { get; set; }

    [Display("Quotation date")] public DateTime QuotationDate { get; set; }

    [Display("Quote ID")] public string QuoteId { get; set; }

    [Display("Request ID")] public string RequestId { get; set; }

    [Display("Status")] public string Status { get; set; }

    [Display("Customer ID")] public string CustomerId { get; set; }

    public RequestResponse(Blackbird.Plugins.Plunet.DataRequest30Service.Request request)
    {
        BriefDescription = request.briefDescription;
        CreationDate = request.creationDate;
        DeliveryDate = request.deliveryDate;
        OrderId = request.orderID.ToString();
        QuotationDate = request.quotationDate;
        QuoteId = request.quoteID.ToString();
        RequestId = request.requestID.ToString();
        Status = request.status.ToString();
        Subject = request.subject;
    }

    public RequestResponse(Blackbird.Plugins.Plunet.DataRequest30Service.Request request, int customerId)
            : this(request)
    {
        CustomerId = customerId.ToString();
    }


}