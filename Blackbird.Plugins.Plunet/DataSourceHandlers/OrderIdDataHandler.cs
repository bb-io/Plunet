using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.Actions;
using Blackbird.Plugins.Plunet.Extensions;

namespace Blackbird.Plugins.Plunet.DataSourceHandlers
{
    public class OrderIdDataHandler : BaseInvocable, IAsyncDataSourceHandler
    {
        private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToList();

        public OrderIdDataHandler(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
            CancellationToken cancellationToken)
        {
            var uuid = Creds.GetAuthToken();
            await using var client = Clients.GetOrderClient(Creds.GetInstanceUrl());
            var orderIds = await client.searchAsync(uuid, new DataOrder30Service.SearchFilter_Order());
            var orders = await client.getOrderObjectListAsync(uuid, orderIds.data);
            await Creds.Logout();

            return orders.OrderListResult.data
                .Where(x => context.SearchString == null ||
                            x.orderDisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase) ||
                            x.projectName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase) ||
                            x.subject.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                .Take(20)
                .ToDictionary(x => x.orderID.ToString(), x => $"{x.orderDisplayName}  {x.subject}");

        }
    }
}
