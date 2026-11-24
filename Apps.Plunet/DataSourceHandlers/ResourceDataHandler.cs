using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class ResourceIdDataHandler(InvocationContext invocationContext) : PlunetInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var allWorkingStatuses = new int?[] { 1, 2 };
        var allStatuses = new int?[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var resources = await ExecuteWithRetry(() => ResourceClient.getAllResourceObjects2Async(Uuid, allWorkingStatuses, allStatuses));

        return resources
            .Where(x => context.SearchString == null ||
                        x.ToDisplayName().Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.resourceID.ToString(), x.ToDisplayName()))
            .ToList();
    }
}