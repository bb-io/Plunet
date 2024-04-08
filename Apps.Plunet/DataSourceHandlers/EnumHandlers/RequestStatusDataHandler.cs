using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class RequestStatusDataHandler : IStaticDataSourceHandler
{
    private static Dictionary<string, string> EnumValues => new()
    {
        { "1", "In preparation" },
        { "2", "Pending" },
        { "5", "Canceled" },
        { "6", "Changed into quote" },
        { "7", "Changed into order" },
        { "8", "New auto" },
        { "9", "Rejected" },
    };
    
    public Dictionary<string, string> GetData()
    {
        return EnumValues;
    }
}