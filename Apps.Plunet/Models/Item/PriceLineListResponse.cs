using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Item;

public class PriceLineListResponse
{
    [Display("Price lines")]
    public IEnumerable<PriceLineResponse> PriceLines { get; set; }
}