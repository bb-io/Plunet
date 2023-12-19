using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Item;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers
{
    public class ItemPriceUnitDataHandler : PlunetInvocable, IAsyncDataSourceHandler
    {
        private ItemPriceUnitRequest request;
        public ItemPriceUnitDataHandler(InvocationContext invocationContext, [ActionParameter] ItemPriceUnitRequest context) : base(invocationContext)
        {
            request = context;
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Service))
                throw new("Please fill in the service first");

            var response = await ItemClient.getPriceUnit_ListAsync(Uuid, Language, request.Service);

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            return response.data
                .Where(unit => context.SearchString == null ||
                                   unit.description.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(x => x.priceUnitID.ToString(), x => x.description);
        }
    }
}
