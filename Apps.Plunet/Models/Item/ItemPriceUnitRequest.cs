using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Item
{
    public class ItemPriceUnitRequest
    {
        [Display("Service")]
        [DataSource(typeof(ServiceNameDataHandler))]
        public string Service { get; set; }

        [Display("Price unit")]
        [DataSource(typeof(ItemPriceUnitDataHandler))]
        public string PriceUnit { get; set; }
    }
}
