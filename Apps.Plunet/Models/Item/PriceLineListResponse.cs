using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Item;

public class PriceLineListResponse
{
    [Display("Price lines")]
    public IEnumerable<PriceLineResponse> PriceLines { get; set; }
}