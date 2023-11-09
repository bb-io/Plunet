using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Payable.Response
{
    public class PayableItemsResponse
    {
        [Display("Items")]
        public IEnumerable<PayableItemResponse> Items { get; set; }
    }
}
