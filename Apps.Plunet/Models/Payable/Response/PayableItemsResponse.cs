using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.Payable.Response
{
    public class PayableItemsResponse
    {
        [Display("Items")]
        public IEnumerable<PayableItemResponse> Items { get; set; }
    }
}
