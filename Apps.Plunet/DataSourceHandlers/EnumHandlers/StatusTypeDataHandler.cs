using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class StatusTypeDataHandler : IStaticDataSourceHandler
    {
        private static Dictionary<string, string> EnumValues => new()
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

        public Dictionary<string, string> GetData()
        {
            return EnumValues;
        }
    }
}
