using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class ProjectManagerIdDataHandler : PlunetInvocable, IAsyncDataSourceHandler
{
    public ProjectManagerIdDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        // Only active & internal resources
        var resources = await ResourceClient.getAllResourceObjects2Async(Uuid, new int?[] { 1 }, new int?[] { 1, 4, 6, 5, 7, 8 });

        return resources.ResourceListResult.data
            .Where(x => (context.SearchString == null ||
                         x.name1.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase)) && x.resourceType > 1) //2 - project manager, 3 - supervisor
            .Take(20)
            .ToDictionary(x => x.resourceID.ToString(), x => x.fullName);
    }
}