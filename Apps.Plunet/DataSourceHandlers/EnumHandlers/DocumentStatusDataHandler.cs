using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class DocumentStatusDataHandler : EnumDataHandler
    {
        protected override Dictionary<string, string> EnumValues => new()
        {
            { "7", "Documents approved" },
            { "1", "Documents available" },
            { "2", "Documents downloaded" },
            { "3", "Documents in review" },
            { "6", "Documents re-delivered" },
            { "0", "No documents available" },
            { "5", "Post processing in progress" },
            { "4", "Post processting requested" }
        };
    }
}
