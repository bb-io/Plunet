using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class JobFeedbackCriteriaDataHandler(InvocationContext invocationContext)
    : PlunetInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var criteria = await AdminClient.getJobFeedbackCriteriaAsync(Uuid);

        return criteria.data
            .Where(x => x.active)
            .Select(x => new DataSourceItem(x.id.ToString(), $"{x.label} ({x.id})"));
    }
}
