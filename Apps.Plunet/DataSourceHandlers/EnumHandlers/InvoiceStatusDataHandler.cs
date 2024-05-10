using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class InvoiceStatusDataHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "0", "Undefined" },
            { "1", "Outstanding" },
            { "2", "Paid" },
            { "3", "Canceled" },
            { "4", "Reminder created" },
            { "5", "In preparation" },
            { "6", "Uncollected" },
            { "7", "Cancellation voucher" }
        };
    }
}