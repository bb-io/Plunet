using Apps.Plunet.Actions;
using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class PayableDataSourceHandler(InvocationContext invocationContext)
    : PlunetInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var invoiceActions = new PayableActions(InvocationContext, null);
        var invoices = await invoiceActions.SearchPayables(new ());
        
        return invoices.Items
            .Where(x => context.SearchString == null ||
                        x.ExternalInvoiceNumber.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(20)
            .ToDictionary(x => x.Id, x => x.ExternalInvoiceNumber);
    }
}