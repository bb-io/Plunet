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

    [Display("Request number")] public string RequestNumber { get; set; }

    [Display("Customer reference number")] public string CustomerRefNo { get; set; }

    [Display("Customer contact ID")] public string CustomerContactId { get; set; }

    public RequestResponse(Blackbird.Plugins.Plunet.DataRequest30Service.Request request, 
        string customerId, 
        string requestNumber,
        string customerRefNo,
        string customerContactId)
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
        CustomerId = customerId.ToString();
        RequestNumber = requestNumber;
        CustomerRefNo = customerRefNo;
        CustomerContactId = customerContactId;
    }
}
