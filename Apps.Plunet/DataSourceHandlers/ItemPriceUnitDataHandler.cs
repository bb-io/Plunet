using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Item;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class ItemPriceUnitDataHandler(InvocationContext invocationContext, [ActionParameter] ItemPriceUnitRequest request)
    : PlunetInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Service))
            throw new PluginMisconfigurationException("Please fill in the service first");

        var response = await ExecuteWithRetry(() => AdminClient.getAvailablePriceUnitsAsync(Uuid, Language, request.Service));
        return response
            .Where(unit => context.SearchString == null ||
                           unit.description.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.priceUnitID.ToString(), x.description))
            .ToList();
    }
}