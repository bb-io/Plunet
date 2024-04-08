using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class CustomerStatusDataHandler : IStaticDataSourceHandler
    {
        public Dictionary<string, string> GetData()
        {
            return new()
            {
                ["1"] = "Active",
                ["6"] = "New (registered)",
                ["5"] = "Blocked",
                ["3"] = "Contacted",
                ["8"] = "Deletion requested",
                ["4"] = "New",
                ["7"] = "New auto",
                ["2"] = "Not active",
            };
        }
    }
}