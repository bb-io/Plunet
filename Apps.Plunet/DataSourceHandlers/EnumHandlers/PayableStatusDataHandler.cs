using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class PayableStatusDataHandler : IStaticDataSourceHandler
{
    private static Dictionary<string, string> EnumValues => new()
    {
        { "1", "Outstanding" },
        { "2", "Paid" },
        { "3", "Canceled" },
        { "4", "Created by external user" },
        { "5", "In preparation" },
        { "6", "Invoice checked" },
    };

    public Dictionary<string, string> GetData()
    {
        return EnumValues;
    }
}