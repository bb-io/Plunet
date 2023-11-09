using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class PayableStatusDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "1", "Outstanding" },
        { "2", "Paid" },
        { "3", "Canceled" },
        { "4", "Created by external user" },
        { "5", "In preparation" },
        { "6", "Invoice checked" },
    };
}