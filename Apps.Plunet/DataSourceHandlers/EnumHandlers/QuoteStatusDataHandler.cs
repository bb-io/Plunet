using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class QuoteStatusDataHandler : IStaticDataSourceHandler
{
    private static Dictionary<string, string> EnumValues => new()
    {
        { "1", "Pending" },
        { "2", "Revised" },
        { "3", "Rejected" },
        { "4", "Change into order" },
        { "5", "Canceled" },
        { "6", "Expired" },
        { "7", "Accepted" },
        { "8", "New" },
        { "9", "In preparation" },
        { "11", "Check clearance" }
    };

    public Dictionary<string, string> GetData()
    {
        return EnumValues;
    }
}