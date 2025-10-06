using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Item;

public class ItemResponse
{
    [Display("Description")]
    public string BriefDescription { get; set; }

    [Display("Comment")]
    public string Comment { get; set; }

    [Display("Delivery deadline")]
    public DateTime? DeliveryDeadline { get; set; }

    [Display("Invoice ID")]
    public string InvoiceId { get; set; }

    [Display("Item ID")]
    public string ItemId { get; set; }

    [Display("Jobs IDs")]
    public IEnumerable<string> Jobs { get; set; }

    [Display("Order ID")]
    public string OrderId { get; set; }

    [Display("Project ID")]
    public string ProjectId { get; set; }

    [Display("Project Type")]
    public string ProjectType { get; set; }

    [Display("Reference")]
    public string Reference { get; set; }

    [Display("Source language")]
    public string SourceLanguage { get; set; }

    [Display("Target language")]
    public string TargetLanguage { get; set; }

    [Display("Status")]
    public string Status { get; set; }

    [Display("Total price")]
    public double TotalPrice { get; set; }

    [Display("Tax type")]
    public string TaxType { get; set; }

    public ItemResponse(Blackbird.Plugins.Plunet.DataItem30Service.Item item, ItemProjectType projectType) 
    {
        BriefDescription = item.briefDescription;
        Comment = item.comment;
        if (item.deliveryDeadlineSpecified)
        {
            DeliveryDeadline = item.deliveryDeadline;
        }
        InvoiceId = item.invoiceID.ToString();
        ItemId = item.itemID.ToString();
        Jobs = item.jobIDList == null ? new List<string> { } : item.jobIDList.Where(x => x.HasValue).Select(x => x.ToString()!);
        OrderId = projectType == ItemProjectType.Order ? item.projectID.ToString() : item.orderID.ToString();
        ProjectId = item.projectID.ToString();
        ProjectType = item.projectType.ToString();
        Reference = item.reference;
        SourceLanguage = item.sourceLanguage;
        TargetLanguage = item.targetLanguage;
        Status = item.status.ToString();
        TotalPrice = item.totalPrice;
        TaxType = item.taxType.ToString();
    }
}