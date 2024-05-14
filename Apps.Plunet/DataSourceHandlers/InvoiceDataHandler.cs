using Apps.Plunet.Actions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Invoices;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class InvoiceDataHandler(InvocationContext invocationContext)
    : PlunetInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var invoiceActions = new InvoiceActions(InvocationContext, null);
        var invoices = await invoiceActions.SearchInvoices(new SearchInvoicesRequest());
        
        return invoices.Invoices
            .Where(x => context.SearchString == null ||
                        x.ProjectName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(20)
            .ToDictionary(x => x.InvoiceId, x => x.ProjectName);
    }
}