using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers
{
    public class QuoteTemplateDataHandler : PlunetInvocable, IAsyncDataSourceHandler
    {
        public QuoteTemplateDataHandler(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
            CancellationToken cancellationToken)
        {
            var response = await QuoteClient.getTemplateListAsync(Uuid);

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            return response.data
                .Where(x => context.SearchString == null ||
                            x.templateName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(x => x.templateID.ToString(), x => x.templateName);
        }
    }
}