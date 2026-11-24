using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Item;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class ProjectDataSourceHandler(InvocationContext invocationContext, [ActionParameter] OptionalItemProjectRequest actionContext)
    : PlunetInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(actionContext.ProjectType))
            throw new("Please fill in the type first");

        if (actionContext.ProjectType == "3") // order
        {
            var orderDataHandler = new OrderIdDataHandler(InvocationContext);
            return await orderDataHandler.GetDataAsync(context, cancellationToken);
        }

        var quoteDataHandler = new QuoteIdDataHandler(InvocationContext);
        return await quoteDataHandler.GetDataAsync(context, cancellationToken);            
    }
}