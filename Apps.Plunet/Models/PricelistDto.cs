using Blackbird.Applications.Sdk.Common;
using Blackbird.Plugins.Plunet.DataItem30Service;

namespace Apps.Plunet.Models
{
    public class PricelistDto
    {
        [Display("Pricelist ID")]
        public string Id { get; set; }

        [Display("Resource Pricelist ID")]
        public string ResourcePricelistId { get; set; }

        public string Name { get; set; }

        public string Currency { get; set; }

        public string Memo { get; set; }

        public PricelistDto(Pricelist input) 
        {
            Id = input.pricelistID.ToString();
            ResourcePricelistId = input.ResourcePricelistID.ToString();
            Name = input.PricelistNameEN;
            Currency = input.currency;
            Memo = input.memo;

        }
    }
}
