using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class TimeFrameRelationDataHandler : IStaticDataSourceHandler
{
    private static Dictionary<string, string> EnumValues => new()
    {
        { "1", "Invoice date" },
        { "2", "Value date" },
        { "3", "Due date" },
        { "4", "Paid date" },
    };

    public Dictionary<string, string> GetData()
    {
        return EnumValues;
    }
}