using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Plugins.Plunet.DataSourceHandlers;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class UpdateOrderRequest : CreateOrderRequest
{
    [Display("Order ID")]
    [DataSource(typeof(OrderIdDataHandler))]
    public string OrderId { get; set; }
}