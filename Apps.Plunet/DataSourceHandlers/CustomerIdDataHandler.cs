using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class CustomerIdDataHandler(InvocationContext invocationContext) : PlunetInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    private static readonly int?[] AllStatuses = [1, 2, 3, 4, 5, 6, 7, 8];
    
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var response = await ExecuteWithRetry(() => CustomerClient.getAllCustomerObjects2Async(Uuid, AllStatuses));

        return response
            .Where(x => context.SearchString == null ||
                        x.fullName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.customerID.ToString(), x.fullName))
            .ToList();
    }
}