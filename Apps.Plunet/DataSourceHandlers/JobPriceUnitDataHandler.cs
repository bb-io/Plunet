using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common;
using Apps.Plunet.Models.Job;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Plunet.DataSourceHandlers;

public class JobPriceUnitDataHandler(InvocationContext invocationContext, [ActionParameter] JobPriceUnitRequest request)
    : PlunetInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Service))
            throw new PluginMisconfigurationException("Please fill in the service first");

        var response = await ExecuteWithRetry(() => AdminClient.getAvailablePriceUnitsAsync(Uuid, Language, request.Service));
        var data = response.DistinctBy(x => x.priceUnitID).ToList();

        return data
            .Where(unit => string.IsNullOrEmpty(context.SearchString) ||
                           unit.description.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.priceUnitID.ToString(), x.description ?? "[Empty]"));
    }
}