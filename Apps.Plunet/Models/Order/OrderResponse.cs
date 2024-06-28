using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Order;

public class OrderResponse
{
    [Display("Currency")]
    public string Currency { get; set; }

    [Display("Customer ID")]
    public string? CustomerId { get; set; }

    [Display("Contact ID")]
    public string? ContactId { get; set; }

    [Display("Delivery deadline")]
    public DateTime DeliveryDeadline { get; set; }

    [Display("Order closing date")]
    public DateTime OrderClosingDate { get; set; }

    [Display("Order date")]
    public DateTime OrderDate { get; set; }

    [Display("Order name")]
    public string OrderName { get; set; }

    [Display("Order ID")]
    public string OrderId { get; set; }

    [Display("Project manager ID")]
    public string ProjectManagerId { get; set; }

    [Display("Project name")]
    public string ProjectName { get; set; }

    [Display("Rate")]
    public double Rate { get; set; }

    [Display("Language combinations")]
    public IEnumerable<LanguageCombination> LanguageCombinations { get; set; }

    [Display("Target languages")]
    public IEnumerable<string> AllTargetLanguages { get; set; }

    [Display("Source languages")]
    public IEnumerable<string> AllSourceLanguages { get; set; }

    [Display("Total price")]
    public double TotalPrice { get; set; }

    [Display("Status")]
    public string Status { get; set; }
    
    [Display("Request ID")]
    public string RequestId { get; set; }

    [Display("Project category")]
    public string ProjectCategory { get; set; }
    
    [Display("Project status")] 
    public string ProjectStatus { get; set; }

    public OrderResponse(Blackbird.Plugins.Plunet.DataOrder30Service.Order order, IEnumerable<LanguageCombination> combinations)
    {
        Currency = order.currency;
        CustomerId = order.customerID == 0 ? null : order.customerID.ToString();
        DeliveryDeadline = order.deliveryDeadline;
        OrderClosingDate = order.orderClosingDate;
        OrderDate = order.orderDate;
        OrderId = order.orderID.ToString();
        OrderName = order.orderDisplayName;
        ProjectManagerId = order.projectManagerID.ToString();
        ProjectName = order.projectName;
        Rate = order.rate;
        LanguageCombinations = combinations;
        AllTargetLanguages = combinations == null || !combinations.Any() ? new List<string>() : combinations.Select(x => x.Target).Distinct();
        AllSourceLanguages = combinations == null || !combinations.Any() ? new List<string>() : combinations.Select(x => x.Source).Distinct();
        RequestId = order.requestID.ToString();
    }
}