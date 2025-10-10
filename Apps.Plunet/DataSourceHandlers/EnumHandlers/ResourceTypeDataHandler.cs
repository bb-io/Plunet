using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class ResourceTypeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new DataSourceItem("0", "Resources"),
            new DataSourceItem("1", "Team member"),
            new DataSourceItem("2", "Project manager"),
            new DataSourceItem("3", "Supervisor"),
        };
    }
}
