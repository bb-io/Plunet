using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class JobRoundStatusDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new DataSourceItem("0", "In preparation"),
            new DataSourceItem("1", "Requested"),
            new DataSourceItem("2", "Assignment error"),
            new DataSourceItem("3", "Job assigned"),
            new DataSourceItem("4", "No assignment"),
            new DataSourceItem("5", "Canceled"),
            new DataSourceItem("6", "Reaction time elapsed"),
            new DataSourceItem("7", "Review"),
            new DataSourceItem("8", "Canceled early"),
        };
    }
}
