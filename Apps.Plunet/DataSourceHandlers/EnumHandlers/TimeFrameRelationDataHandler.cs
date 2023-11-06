using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Blackbird.Plugins.Plunet.DataSourceHandlers.EnumHandlers;

public class TimeFrameRelationDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "1", "Invoice date" },
        { "2", "Value date" },
        { "3", "Due date" },
        { "4", "Paid date" },
    };
}