using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Order
{
    public class OrderNumberRequest
    {
        [Display("Order number")]
        public string OrderNumber { get; set; }
    }
}
