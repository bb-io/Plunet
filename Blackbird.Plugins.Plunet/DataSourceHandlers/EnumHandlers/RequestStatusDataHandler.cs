//using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Blackbird.Plugins.Plunet.DataSourceHandlers.EnumHandlers;

public class RequestStatusDataHandler : BaseInvocable, IDataSourceHandler /*: EnumDataHandler*/
{
    protected Dictionary<string, string> EnumValues => new()
    {
        { "1", "In preparation" },
        { "2", "Pending" },
        { "5", "Canceled" },
        { "6", "Changed into quote" },
        { "7", "Changed into order" },
        { "8", "New auto" },
        { "9", "Rejected" },
    };

    public RequestStatusDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public Dictionary<string, string> GetData(DataSourceContext context)
    {
        return EnumValues;
    }
}