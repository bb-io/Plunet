using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.Order
{
    public class ListOrderResponse
    {
        public IEnumerable<OrderResponse> Orders { get; set; }
    }
}
