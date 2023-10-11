using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Invocables;
using Blackbird.Plugins.Plunet.Models.Order;

namespace Blackbird.Plugins.Plunet.DataSourceHandlers;

public class TemplateDataHandler : PlunetInvocable, IAsyncDataSourceHandler
{
    public TemplateDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var uuid = Creds.GetAuthToken();

        await using var orderClient = Clients.GetOrderClient(Creds.GetInstanceUrl());
        var response = await orderClient.getTemplateListAsync(uuid);

        await Creds.Logout();

        var templates = response.data.Select(x => new TemplateResponse(x)).ToArray();

        return templates
            .Where(x => context.SearchString == null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(x => x.Id, x => x.Name);
    }
}