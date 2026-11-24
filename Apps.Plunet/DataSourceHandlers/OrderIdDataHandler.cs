using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class OrderIdDataHandler(InvocationContext invocationContext) : PlunetInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var orderIds = await ExecuteWithRetryAcceptNull(() => OrderClient.searchAsync(Uuid, new()
        {                
            projectName = context.SearchString ?? ""
        }));

        if (orderIds is null || orderIds.Length == 0)
            return [];

        var orders = await ExecuteWithRetry(() => OrderClient.getOrderObjectListAsync(Uuid, orderIds));
        return orders.Select(x => new DataSourceItem(x.orderID.ToString(), x.ToDisplayName())).ToList();
    }
}
