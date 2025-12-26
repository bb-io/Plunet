using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Request.Response;

public class RequestResponse
{
    [Display("Brief description")] public string BriefDescription { get; set; }

    [Display("Subject")] public string Subject { get; set; }

    [Display("Creation date")] public DateTime CreationDate { get; set; }

    [Display("Delivery date")] public DateTime DeliveryDate { get; set; }

    [Display("Order ID")] public string OrderId { get; set; }

    [Display("First Order ID List")] public string FirstOrderIDList { get; set; }

    [Display("Quotation date")] public DateTime QuotationDate { get; set; }

    [Display("Quote ID")] public string QuoteId { get; set; }

    [Display("First Quote ID List")] public string FirstQuoteIDList { get; set; }

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
        FirstOrderIDList = (request.orderIDList != null) ? request.orderIDList[0].ToString() : "0";
        QuotationDate = request.quotationDate;
        QuoteId = request.quoteID.ToString();
        FirstQuoteIDList = (request.quoteIDList != null) ? request.quoteIDList[0].ToString() : "0";
        RequestId = request.requestID.ToString();
        Status = request.status.ToString();
        Subject = request.subject;
        CustomerId = customerId.ToString();
        RequestNumber = requestNumber;
        CustomerRefNo = customerRefNo;
        CustomerContactId = customerContactId;
    }
}
