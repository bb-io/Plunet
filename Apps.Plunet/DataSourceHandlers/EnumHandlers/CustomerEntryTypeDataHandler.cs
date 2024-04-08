using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class CustomerEntryTypeDataHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            ["1"] = "Customer",
            ["3"] = "Indirect customer",
            ["7"] = "Account manager",
        };
    }
}