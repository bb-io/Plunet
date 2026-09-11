using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class FormatDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new[]
        {
            new DataSourceItem("0", "Standard"),
            new DataSourceItem("1", "Abbreviated version"),
            new DataSourceItem("2", "Weighted quality"),
            new DataSourceItem("3", "TM discount"),
            new DataSourceItem("4", "Quantity not weighted"),
            new DataSourceItem("5", "Standard with price memo"),
            new DataSourceItem("6", "Output as in order"),
        };
    }
}