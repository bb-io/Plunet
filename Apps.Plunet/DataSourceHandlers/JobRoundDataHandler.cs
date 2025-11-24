using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Job;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class JobRoundDataHandler(InvocationContext context, [ActionParameter] GetJobRequest job) 
    : PlunetInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(job.JobId) || string.IsNullOrEmpty(job.ProjectType))
            throw new PluginMisconfigurationException("Please specify job ID and project type first");

        var result = await ExecuteWithRetry(async () => 
            await JobRoundClient.getAllRoundIDsAsync(Uuid, ParseId(job.JobId), ParseId(job.ProjectType))
        );
        return result.Select(x => new DataSourceItem(x.Value.ToString(), x.Value.ToString()));
    }
}
