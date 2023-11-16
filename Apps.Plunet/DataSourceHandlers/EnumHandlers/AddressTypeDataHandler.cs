using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class AddressTypeDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "1", "Shipping address" },
        { "2", "Billing address" },
        { "3", "Other" },
    };
}