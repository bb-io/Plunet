using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class TextModuleUsageDataHandler : EnumDataHandler
    {
        protected override Dictionary<string, string> EnumValues => new()
        {
            { "1", "Customer" },
            { "6", "Order" },
            { "11", "Order job" },
            { "8", "Payable" },
            { "5", "Quote" },
            { "10", "Quote job" },
            { "7", "Receivable" },
            { "9", "Receivable credit note" },
            { "4", "Request" },
            { "2", "Resource" },
            { "3", "Vendor" }
        };
    }
}
