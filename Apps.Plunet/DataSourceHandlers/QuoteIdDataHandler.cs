using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class QuoteIdDataHandler(InvocationContext invocationContext) : PlunetInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var quoteIds = await ExecuteWithRetry(() => QuoteClient.searchAsync(Uuid, new()));
        if (quoteIds.Length == 0)
            return [];
        
        var quotes = await ExecuteWithRetry(() => QuoteClient.getQuoteObjectListAsync(Uuid, quoteIds));
        return quotes
            .Where(x => context.SearchString == null || x.projectName.Contains(context.SearchString))
            .Select(x => new DataSourceItem(x.quoteID.ToString(), x.projectName))
            .ToList();
    }
}