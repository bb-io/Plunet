using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class DateRelationDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new DataSourceItem("1", "Date of order"),
            new DataSourceItem("2", "Item creation date"),
            new DataSourceItem("3", "Order due date"),
            new DataSourceItem("4", "Item due date"),
            new DataSourceItem("5", "Item delivered on"),
            new DataSourceItem("6", "Installment date"),
        };
    }
}
