using Apps.Plunet.Invocables;
using Apps.Plunet.Models.;
using Apps.Plunet.Models.Item;
using Apps.Plunet.Webhooks.CallbackClients;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.DataSourceHandlers
{
    public class ProjectDataSourceHandler : PlunetInvocable, IAsyncDataSourceHandler
    {
        private ItemProjectRequest actionContext;
        public ProjectDataSourceHandler(InvocationContext invocationContext, [ActionParameter] ItemProjectRequest context) : base(invocationContext)
        {
            actionContext = context;
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(actionContext.ProjectType))
                throw new("Please fill in the type first");

            if (actionContext.ProjectType == "3") // order
            {
                var orderDataHandler = new OrderIdDataHandler(InvocationContext);
                return await orderDataHandler.GetDataAsync(context, cancellationToken);
            }

            var quoteDataHandler = new QuoteIdDataHandler(InvocationContext);
            return await quoteDataHandler.GetDataAsync(context, cancellationToken);            
        }
    }
}
