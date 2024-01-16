using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class PropertyUsageDataHandler : EnumDataHandler
    {
        protected override Dictionary<string, string> EnumValues => new()
        {
            { "1", "Customer" },
            { "6", "Order" },
            { "10", "Order item" },
            { "12", "Order job" },
            { "5", "Quote" },
            { "9", "Quote item" },
            { "11", "Quote job" },
            { "4", "Request" },
            { "2", "Resource" },
        };
    }
}
