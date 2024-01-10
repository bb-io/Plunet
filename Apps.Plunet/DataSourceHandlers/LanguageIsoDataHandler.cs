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
        var languages = await GetSystemLanguages();

        return languages
            .Where(x => !string.IsNullOrWhiteSpace(x.isoCode))
            .Where(language => context.SearchString == null ||
                               language.name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .GroupBy(x => x.isoCode)
            .ToDictionary(x => x.Key.ToString(),
                x => string.Join(", ", x.Select(x => x.name)));
    }
}