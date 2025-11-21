using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Job;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class RoundResourceDataHandler(InvocationContext context, [ActionParameter] JobRoundRequest round)
    : PlunetInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(round.JobRoundId))
            throw new PluginMisconfigurationException("Please specify job round ID first");

        var result = await ExecuteWithRetry(async () => 
            await JobRoundClient.getResourcesForRoundAsync(Uuid, ParseId(round.JobRoundId))
        );
        return result.Select(x => new DataSourceItem(x!.Value.ToString(), x.Value.ToString()));
    }
}
