using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class PropertyUsageDataHandler : IStaticDataSourceHandler
    {
        private static Dictionary<string, string> EnumValues => new()
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

        public Dictionary<string, string> GetData()
        {
            return EnumValues;
        }
    }
}
