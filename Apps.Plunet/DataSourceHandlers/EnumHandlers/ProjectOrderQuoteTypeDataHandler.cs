using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class ProjectOrderQuoteTypeDataHandler : IStaticDataSourceHandler
{
    private static Dictionary<string, string> EnumValues => new()
    {
        { "1", "Quote" },
        { "3", "Order" }
    };

    public Dictionary<string, string> GetData()
    {
        return EnumValues;
    }
}