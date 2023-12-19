using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models
{
    public class PricelinesResponse
    {
        [Display("Pricelines")]
        public IEnumerable<PricelineResponse> Pricelines { get; set; }
    }
}
