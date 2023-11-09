using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Order;

public class CreateOrderResponse
{
    [Display("Order ID")]
    public string OrderId { get; set; }
}