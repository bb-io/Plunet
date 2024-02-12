using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class RequestDataHandler : PlunetInvocable, IAsyncDataSourceHandler
{
    public RequestDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var response = await RequestClient.getAll_RequestsAsync(Uuid);

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        return response.data
            .Where(x => string.IsNullOrEmpty(context.SearchString) ||
                            (x.HasValue && MapToReadableString(x.Value).Contains(context.SearchString, StringComparison.OrdinalIgnoreCase)))
            .ToDictionary(x => x.ToString(), x => MapToReadableString(x.Value));
    }
    
    private string MapToReadableString(int requestId)
    {
        return $"R-{requestId:D5}";
    }
}