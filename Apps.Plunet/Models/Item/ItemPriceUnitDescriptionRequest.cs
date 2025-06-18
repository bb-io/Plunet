using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Item
{
    public class ItemPriceUnitDescriptionRequest
    {
        [Display("Service")]
        [DataSource(typeof(ServiceNameDataHandler))]
        public string Service { get; set; }

        [Display("Price unit description")]
        [DataSource(typeof(ItemPriceUnitDescriptionHandler))]
        public string PriceUnitDescription { get; set; }
    }
}
