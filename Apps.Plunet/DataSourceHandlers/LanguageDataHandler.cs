using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.Api;
using Blackbird.Plugins.Plunet.Extensions;

namespace Blackbird.Plugins.Plunet.DataSourceHandlers;

public class LanguageDataHandler : BaseInvocable, IAsyncDataSourceHandler
{
    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToList();

    public LanguageDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var uuid = Creds.GetAuthToken();
          
        await using var client = Clients.GetAdminClient(Creds.GetInstanceUrl());
            
        var languages = await client.getAvailableLanguagesAsync(uuid, "en");

        await Creds.Logout();

        return languages.data
            .Where(language => context.SearchString == null ||
                               language.name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(20)
            .ToDictionary(language => language.folderName, language => language.name);
    }
}