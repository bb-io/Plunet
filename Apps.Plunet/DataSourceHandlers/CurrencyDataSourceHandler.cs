using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.Plunet.Api;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;

namespace Apps.Plunet.DataSourceHandlers
{
    public class CurrencyDataSourceHandler : PlunetInvocable, IAsyncDataSourceHandler
    {
        public CurrencyDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
            CancellationToken cancellationToken)
        {
            var currencies = await AdminClient.getSystemCurrenciesAsync(Uuid);

            return currencies.data
                .Where(x => context.SearchString == null ||
                            x.description.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(x => x.isoCode, x => x.description);
        }
    }
}
