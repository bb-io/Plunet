//using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.Actions;

namespace Blackbird.Plugins.Plunet.DataSourceHandlers.EnumHandlers;

public class AddressTypeDataHandler : BaseInvocable, IDataSourceHandler /*: EnumDataHandler*/
{
    protected Dictionary<string, string> EnumValues => new()
    {
        { "1", "Shipping address" },
        { "2", "Billing address" },
        { "3", "Other" },
    };

    public AddressTypeDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public Dictionary<string, string> GetData(DataSourceContext context)
    {
        return EnumValues;
    }
}