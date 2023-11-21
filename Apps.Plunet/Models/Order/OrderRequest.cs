using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Order
{
    public class OrderRequest
    {
        [Display("Order")]
        [DataSource(typeof(OrderIdDataHandler))]
        public string OrderId { get; set; }
    }
}
