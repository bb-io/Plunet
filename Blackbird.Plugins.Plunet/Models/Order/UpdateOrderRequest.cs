using Blackbird.Applications.Sdk.Common;
using System.ComponentModel.DataAnnotations;
using DisplayAttribute = Blackbird.Applications.Sdk.Common.DisplayAttribute;

namespace Blackbird.Plugins.Plunet.Models.Quote;

public class UpdateOrderRequest
{
    [Display("Order ID")]
    public int OrderId { get; set; }

    [Display("Currency")]
    public string Currency { get; set; }

    [Display("Customer ID")]
    public int CustomerId { get; set; }

    [Display("Project manager Memo")]
    public string ProjectManagerMemo { get; set; }

    [Display("Project name")]
    public string ProjectName { get; set; }

    [Display("Reference number")]
    public string ReferenceNumber { get; set; }

    [Display("Subject")]
    public string Subject { get; set; }

    [Display("Status")]
    public int Status { get; set; }

    [Display("Delivery deadline date")]
    public DateTime DeliveryDeadline { get; set; }

}