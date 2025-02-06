using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers
{
    public class LanguageNameDataHandler : PlunetInvocable, IAsyncDataSourceHandler
    {
        public LanguageNameDataHandler(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var languages = await GetSystemLanguages();

            return languages
            .Where(x => !string.IsNullOrWhiteSpace(x.name))
            .Where(language => context.SearchString == null ||
                             language.name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .GroupBy(x => x.name)
            .ToDictionary(x => x.Key, x => x.FirstOrDefault()?.name ?? "");
        }
    }
}
