using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class LanguageIsoDataHandler : PlunetInvocable, IAsyncDataSourceHandler
{
    public LanguageIsoDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
            
        var languages = await AdminClient.getAvailableLanguagesAsync(Uuid, Language);


        return languages.data
            .Where(language => context.SearchString == null ||
                               language.name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(20)
            .ToDictionary(language => language.isoCode, language => language.name);
    }
}