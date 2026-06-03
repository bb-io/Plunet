using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class JobFeedbackRatingDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new("1", "1 star"),
            new("2", "2 stars"),
            new("3", "3 stars"),
            new("4", "4 stars"),
            new("5", "5 stars")
        ];
    }
}
