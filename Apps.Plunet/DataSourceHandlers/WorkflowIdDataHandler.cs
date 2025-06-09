using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class WorkflowIdDataHandler : PlunetInvocable, IAsyncDataSourceHandler
{
    public WorkflowIdDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var response = await AdminClient.getAvailableWorkflowsAsync(Uuid);

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        return response.data
            .Where(x => context.SearchString == null ||
                        x.name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(x => x.workflowId.ToString(), x => x.name);
    }
}
