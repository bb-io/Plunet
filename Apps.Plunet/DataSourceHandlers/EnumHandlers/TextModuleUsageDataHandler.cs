using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class TextModuleUsageDataHandler : IStaticDataSourceHandler
    {
        private static Dictionary<string, string> EnumValues => new()
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

        public Dictionary<string, string> GetData()
        {
            return EnumValues;
        }
    }
}
