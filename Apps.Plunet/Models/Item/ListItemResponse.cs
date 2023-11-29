using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.Item
{
    public class ListItemResponse
    {
        public IEnumerable<ItemResponse> Items { get; set; }
    }
}
