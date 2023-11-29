using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Item
{
    public class GetItemRequest
    {
        [Display("Item ID")]
        public string ItemId { get; set; }
    }
}
