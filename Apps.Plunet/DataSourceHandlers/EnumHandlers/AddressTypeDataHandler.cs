using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class AddressTypeDataHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "1", "Shipping address" },
            { "2", "Billing address" },
            { "3", "Other" },
        };
    }
}