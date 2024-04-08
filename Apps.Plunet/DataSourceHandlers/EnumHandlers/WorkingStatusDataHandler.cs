using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class WorkingStatusDataHandler : IStaticDataSourceHandler
    {
        private static Dictionary<string, string> EnumValues => new()
        {
            { "1", "Internal" },
            { "2", "External" },
        };

        public Dictionary<string, string> GetData()
        {
            return EnumValues;
        }
    }
}
