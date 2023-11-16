using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Request.Request;

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
    [DataSource(typeof(OrderIdDataHandler))]
    public string? OrderId { get; set; }

    [Display("Quotation date")]
    public DateTime? QuotationDate { get; set; }

    [Display("Quote ID")]
    public string? QuoteId { get; set; }
    
    public string? Service { get; set; }
    
    [Display("Contact")]
    [DataSource(typeof(ContactIdDataHandler))]
    public string? ContactId { get; set; }
    
    [Display("Customer")]
    [DataSource(typeof(CustomerIdDataHandler))]
    public string? CustomerId { get; set; }
    
    [Display("Customer reference number of previous order")]
    public string? ReferenceNumberOfPrev { get; set; }
    
    [Display("Customer reference number")]
    public string? ReferenceNumber { get; set; }
    
    [Display("Is EN10538 standard")]
    public bool? IsEn10538 { get; set; }
    
    [Display("Master project ID")]
    public string? MasterProjectId { get; set; }

    public double? Price { get; set; }
    
    [Display("Is rush request")]
    public bool? IsRushRequest { get; set; }

    [Display("Word count")]
    public int? WordCount { get; set; }

    [Display("Workflow ID")]
    public string? WorkflowId { get; set; }
}