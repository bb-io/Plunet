using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Item;
using Apps.Plunet.Webhooks.CallbackClients;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Plugins.Plunet.DataAdmin30Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Plunet.Models.Job;

namespace Apps.Plunet.DataSourceHandlers
{
    public class JobPriceUnitDataHandler : PlunetInvocable, IAsyncDataSourceHandler
    {
        private JobPriceUnitRequest request;
        public JobPriceUnitDataHandler(InvocationContext invocationContext, [ActionParameter] JobPriceUnitRequest context) : base(invocationContext)
        {
            request = context;
        }

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
                .ToDictionary(x => x.priceUnitID.ToString(), x => x.description);
        }
    }
}
