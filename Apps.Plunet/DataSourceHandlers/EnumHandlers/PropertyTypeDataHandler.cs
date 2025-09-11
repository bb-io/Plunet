using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class PropertyTypeDataHandler : IStaticDataSourceHandler
    {
        public Dictionary<string, string> GetData()
        {
            return new Dictionary<string, string>
            {
                { "1", "Single select" },
                { "2", "Multi select" }
            };
        }
    }
}
