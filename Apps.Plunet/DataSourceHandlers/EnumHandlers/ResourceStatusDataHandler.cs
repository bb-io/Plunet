using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class ResourceStatusDataHandler : IStaticDataSourceHandler
    {
        private static Dictionary<string, string> EnumValues => new()
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

        public Dictionary<string, string> GetData()
        {
            return EnumValues;
        }
    }
}
