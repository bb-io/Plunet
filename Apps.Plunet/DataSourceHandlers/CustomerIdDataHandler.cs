using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.Api;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Invocables;
using Blackbird.Plugins.Plunet.Models.Customer;

namespace Blackbird.Plugins.Plunet.DataSourceHandlers;

public class CustomerIdDataHandler : PlunetInvocable, IAsyncDataSourceHandler
{
    public CustomerIdDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var uuid = Creds.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(Creds.GetInstanceUrl());

        var allStatuses = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var response = await customerClient
            .getAllCustomerObjects2Async(uuid, Array.ConvertAll(allStatuses, i => (int?)i));

        var customers = response.CustomerListResult.data
            .Select(x => new GetCustomerResponse(x)).ToArray();

        await Creds.Logout();

        return customers
            .Where(x => context.SearchString == null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(20)
            .ToDictionary(x => x.CustomerId, x => x.Name);
    }
}