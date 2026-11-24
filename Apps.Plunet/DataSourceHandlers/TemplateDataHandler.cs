using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class TemplateDataHandler(InvocationContext invocationContext) : PlunetInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var response = await ExecuteWithRetry(() => OrderClient.getTemplateListAsync(Uuid));

        return response
            .Where(x => context.SearchString == null ||
                        x.templateName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.templateID.ToString(), x.templateName))
            .ToList();
    }
}