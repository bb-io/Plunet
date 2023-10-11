using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Invocables;

namespace Blackbird.Plugins.Plunet.DataSourceHandlers;

public class ProjectManagerIdDataHandler : PlunetInvocable, IAsyncDataSourceHandler
{
    public ProjectManagerIdDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var uuid = Creds.GetAuthToken();
            
        await using var client = Clients.GetResourceClient(Creds.GetInstanceUrl());
           
        var statuses = new int?[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var resources = await client.getAllResourceObjects2Async(uuid, statuses, statuses);

        return resources.ResourceListResult.data
            .Where(x => (context.SearchString == null ||
                         x.name1.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase)) && x.resourceType == 3 ) // resource type 3 - Project manager
            .Take(20)
            .ToDictionary(x => x.resourceID.ToString(), x => x.fullName);
    }
}