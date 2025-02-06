using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common;
using Apps.Plunet.Models.Job;

namespace Apps.Plunet.DataSourceHandlers
{
    public class JobPriceUnitDataHandler(
        InvocationContext invocationContext,
        [ActionParameter] JobPriceUnitRequest request)
        : PlunetInvocable(invocationContext), IAsyncDataSourceHandler
    {
        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Service))
                throw new("Please fill in the service first");

            var response = await JobClient.getPriceUnit_ListAsync(Uuid, Language, request.Service);

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            return response.data
                .Where(unit => context.SearchString == null ||
                                   unit.description.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(x => x.priceUnitID.ToString(), x => x.description ?? string.Empty);
        }
    }
}
