using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class ItemStatusDataHandler : IStaticDataSourceItemHandler
{
    private static readonly Dictionary<string, string> Items = new()
    {
        { "12", "Accepted" },
        { "3", "Approved" },
        { "5", "Canceled" },
        { "2", "Delivered" },
        { "7", "Can be delivered" },
        { "8", "In preparation" },
        { "1", "In progress" },
        { "4", "Invoiced" },
        { "6", "Auto" },
        { "9", "Paid" },
        { "11", "Pending" },
        { "13", "Rejected" },
        { "14", "Sum" },
        { "10", "Without invoice" }
    };
        
    public IEnumerable<DataSourceItem> GetData()
    {
        return Items.Select(x => new DataSourceItem(x.Key, x.Value));
    }
}