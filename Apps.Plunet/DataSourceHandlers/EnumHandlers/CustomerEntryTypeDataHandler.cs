using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class CustomerEntryTypeDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        ["1"] = "Customer",
        ["3"] = "Indirect customer",
        ["7"] = "Account manager",
    };
}