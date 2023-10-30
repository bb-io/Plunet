using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Blackbird.Plugins.Plunet.DataSourceHandlers.EnumHandlers;

public class TimeFrameRelationDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "1", "Invoice date" },
        { "2", "Valuta" },
        { "3", "Payable until" },
        { "4", "Payed" },
    };
}