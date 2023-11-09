using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class QuoteStatusDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
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
}