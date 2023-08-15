using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.Extensions;

namespace Blackbird.Plugins.Plunet.DataSourceHandlers;

public class CountryDataSourceHandler : BaseInvocable, IAsyncDataSourceHandler
{
    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;

    public CountryDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var uuid = Creds.GetAuthToken();
       
        var client = Clients.GetAdminClient(Creds.GetInstanceUrl());
        var countries = await client.getAvailableCountriesAsync(uuid, "en");
        
        await Creds.Logout();

        return countries.data
            .Where(x => context.SearchString == null ||
                        x.name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.name)
            .Distinct()
            .ToDictionary(x => x, x => x);
    }
}