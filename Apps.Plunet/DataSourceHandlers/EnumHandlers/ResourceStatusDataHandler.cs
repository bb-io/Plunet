using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class ResourceStatusDataHandler : EnumDataHandler
    {
        protected override Dictionary<string, string> EnumValues => new()
        {
            { "1", "Active" },
            { "3", "Blocked" },
            { "10", "Deletion requested" },
            { "9", "Disqualified" },
            { "4", "New" },
            { "6", "New auto" },
            { "2", "Not active or old" },
            { "5", "Premium" },
            { "7", "Probation" },
            { "8", "Qualified" }
        };
    }
}
