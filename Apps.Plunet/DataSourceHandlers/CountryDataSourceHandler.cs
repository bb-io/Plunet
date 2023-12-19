using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class CountryDataSourceHandler : PlunetInvocable, IAsyncDataSourceHandler
{
    public CountryDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var countries = await AdminClient.getAvailableCountriesAsync(Uuid, Language);

        return countries.data
            .Where(x => context.SearchString == null ||
                        x.name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.name)
            .Distinct()
            .ToDictionary(x => x, x => x);
    }
}