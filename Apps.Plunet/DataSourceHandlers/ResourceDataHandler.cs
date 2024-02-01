using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.DataSourceHandlers
{
    public class ResourceIdDataHandler : PlunetInvocable, IAsyncDataSourceHandler
    {
        public ResourceIdDataHandler(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
            CancellationToken cancellationToken)
        {
            var response = await ResourceClient.searchAsync(Uuid, new()
            {
                name1 = context.SearchString ?? ""
            });

            if (response.statusMessage != ApiResponses.Ok)
                throw new("Resource search " + response.statusMessage);

            var allWorkingStatuses = new int?[2] { 1, 2 };
            var allStatuses = new int?[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var resourcesResponse = await ResourceClient.getAllResourceObjects2Async(Uuid, allWorkingStatuses, allStatuses);

            if (resourcesResponse.ResourceListResult.statusMessage != ApiResponses.Ok)
                throw new("Resource name " + resourcesResponse.ResourceListResult.statusMessage);

            return resourcesResponse.ResourceListResult.data
                .Take(20)
                .ToDictionary(x => x.resourceID.ToString(), x => $"{x.name1} {x.name2}");
        }
    }
}
