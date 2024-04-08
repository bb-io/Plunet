using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class CustomerTypeDataHandler : IStaticDataSourceHandler
    {
        public Dictionary<string, string> GetData()
        {
            return new()
            {
                { "0", "Direct" },
                { "2", "Indirect" },
                { "1", "Direct Indirect" },
            };
        }
    }
}
