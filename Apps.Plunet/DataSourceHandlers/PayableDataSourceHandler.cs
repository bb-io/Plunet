using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataPayable30Service;

namespace Apps.Plunet.DataSourceHandlers;

public class PayableDataSourceHandler(InvocationContext invocationContext)
    : PlunetInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var response = await ExecuteWithRetry(async () => await PayableClient.searchAsync(Uuid, new()));

        if (response is null)
        {
            return new List<DataSourceItem>();
        }
        
        return response!
            .Where(x => x.HasValue)
            .Where(x => context.SearchString == null ||
                        x!.Value.ToString() == context.SearchString)
            .Select(x => new DataSourceItem(x!.Value.ToString(), x!.Value.ToString()));
    }
}