using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class CustomerTypeDataHandler : EnumDataHandler
    {
        protected override Dictionary<string, string> EnumValues => new()
        {
            { "0", "Direct" },
            { "2", "Indirect" },
            { "1", "Direct Indirect" },
        };
    }
}
