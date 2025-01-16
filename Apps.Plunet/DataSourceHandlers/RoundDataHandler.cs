using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Job;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataRequest30Service;

namespace Apps.Plunet.DataSourceHandlers
{
    public class RoundDataHandler: PlunetInvocable, IAsyncDataSourceHandler
    {
        private AssignResourceRequest request;

        public RoundDataHandler(InvocationContext invocationContext, [ActionParameter] AssignResourceRequest context) : base(invocationContext)
        {
            request = context;
        }
        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
            CancellationToken cancellationToken)
        {

            var response = await JobRoundClient.getAllRoundIDsAsync(Uuid, ParseId(request.JobId), ParseId(request.ProjectType));

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            return response.data
                .Where(roundId => context.SearchString == null ||
                                  roundId.ToString().Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(roundId => roundId.ToString(), roundId => $"Round {roundId}");
        }
    }
}
