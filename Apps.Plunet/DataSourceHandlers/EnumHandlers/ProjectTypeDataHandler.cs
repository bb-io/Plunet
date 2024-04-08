using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class ProjectTypeDataHandler : IStaticDataSourceHandler
    {
        private static Dictionary<string, string> EnumValues => new()
        {
            { "0", "All" },
            { "2", "Interpreting" },
            { "1", "Translation" },
        };

        public Dictionary<string, string> GetData()
        {
            return EnumValues;
        }
    }
}
