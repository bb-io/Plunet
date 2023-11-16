using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class RequestStatusDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "1", "In preparation" },
        { "2", "Pending" },
        { "5", "Canceled" },
        { "6", "Changed into quote" },
        { "7", "Changed into order" },
        { "8", "New auto" },
        { "9", "Rejected" },
    };
}