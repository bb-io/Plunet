using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class StatusTypeDataHandler : EnumDataHandler
    {
        protected override Dictionary<string, string> EnumValues => new()
        {
            { "1", "Active" },
            { "2", "Inactive" },
            { "3", "Prospect" },
            { "4", "New" },
            { "5", "Blocked" },
            { "6", "Contacted" },
            { "7", "New (registered)" },
            { "8", "Deletion requested" }
        };
    }
}
