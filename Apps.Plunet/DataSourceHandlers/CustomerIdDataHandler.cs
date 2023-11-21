using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Customer;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class CustomerIdDataHandler : PlunetInvocable, IAsyncDataSourceHandler
{
    public CustomerIdDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var allStatuses = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var response = await CustomerClient.getAllCustomerObjects2Async(Uuid, Array.ConvertAll(allStatuses, i => (int?)i));

        if (response.CustomerListResult.statusMessage != ApiResponses.Ok)
            throw new(response.CustomerListResult.statusMessage);

        return response.CustomerListResult.data
            .Where(x => context.SearchString == null ||
                        x.fullName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(20)
            .ToDictionary(x => x.customerID.ToString(), x => x.fullName);
    }
}