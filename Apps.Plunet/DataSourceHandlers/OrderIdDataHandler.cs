using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Webhooks.CallbackClients;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using System;

namespace Apps.Plunet.DataSourceHandlers
{
    public class OrderIdDataHandler : PlunetInvocable, IAsyncDataSourceHandler
    {
        private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToList();

        public OrderIdDataHandler(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
            CancellationToken cancellationToken)
        {

            var response = await OrderClient.searchAsync(Uuid, new()
            {                
                projectName = context.SearchString ?? ""
            });

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            var orders = await OrderClient.getOrderObjectListAsync(Uuid, response.data);

            if (orders.OrderListResult.statusMessage != ApiResponses.Ok)
                throw new(orders.OrderListResult.statusMessage);

            return orders.OrderListResult.data
                .Take(20)
                .ToDictionary(x => x.orderID.ToString(), x => x.projectName);

        }
    }
}
