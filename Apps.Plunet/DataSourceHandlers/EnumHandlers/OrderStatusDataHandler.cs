using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class OrderStatusDataHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "1", "Active" },
            { "3", "Archived" },
            { "6", "Completed" },
            { "2", "Completed archivable" },
            { "5", "In preparation" },
            { "4", "Quote moved to order" }
        };
    }
}