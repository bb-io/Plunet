using Blackbird.Applications.Sdk.Common;
using Blackbird.Plugins.Plunet.DataPayable30Service;

namespace Blackbird.Plugins.Plunet.Models.Payable.Response;

public class PayableResponse
{
    [Display("Payable item ID")] public string PayableItemId { get; set; }

    [Display("Brief description")] public string BriefDescription { get; set; }

    [Display("Invoice ID")] public string InvoiceId { get; set; }

    [Display("Total price")] public double TotalPrice { get; set; }

    [Display("Job date")] public DateTime JobDate { get; set; }

    public PayableResponse(PayableItem payable)
    {
        BriefDescription = payable.briefDescription;
        InvoiceId = payable.invoiceID.ToString();
        JobDate = payable.jobDate;
        PayableItemId = payable.payableItemID.ToString();
        TotalPrice = payable.totalprice;
    }
}