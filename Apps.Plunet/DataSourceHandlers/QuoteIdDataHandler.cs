using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers
{
    public class QuoteIdDataHandler : PlunetInvocable, IAsyncDataSourceHandler
    {
        private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToList();

        public QuoteIdDataHandler(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
            CancellationToken cancellationToken)
        {

            var response = await QuoteClient.searchAsync(Uuid, new());

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            var quotes = await QuoteClient.getQuoteObjectListAsync(Uuid, response.data);

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            if (quotes.QuoteListResult.statusMessage != ApiResponses.Ok)
                throw new(quotes.QuoteListResult.statusMessage);

            return quotes.QuoteListResult.data
                .Where(x => context.SearchString == null || x.projectName.Contains(context.SearchString))    
                .Take(20)
                .ToDictionary(x => x.quoteID.ToString(), x => x.projectName);

        }
    }
}
