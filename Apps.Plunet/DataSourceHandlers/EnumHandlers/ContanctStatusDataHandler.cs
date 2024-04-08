using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class ContanctStatusDataHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "1", "Active" },
            { "2", "Not active" },
            { "3", "Contacted" },
            { "4", "Deletion requested" },
        };
    }
}