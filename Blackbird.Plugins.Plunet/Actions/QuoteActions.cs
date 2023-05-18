using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataAdmin30Service;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using Blackbird.Plugins.Plunet.DataQuote30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Item;
using Blackbird.Plugins.Plunet.Models.Order;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class QuoteActions
{
    [Action("Get quote", Description = "Get details for a Plunet quote")]
    public async Task<QuoteResponse> GetQuote(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int quoteId)
    {
        var uuid = authProviders.GetAuthToken();
        using var quoteClient = Clients.GetQuoteClient(authProviders.GetInstanceUrl());
        var quoteResult = await quoteClient.getQuoteObjectAsync(uuid, quoteId);
        var response = quoteResult.data ?? null;
        await authProviders.Logout();
        return MapQuoteResponse(response);
    }

    [Action("Create quote", Description = "Create a new quote in Plunet")]
    public async Task<CreateQuoteResponse> CreateQuote(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] CreateQuoteRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        using var quoteClient = Clients.GetQuoteClient(authProviders.GetInstanceUrl());
        var quoteIdResult = await quoteClient.insert2Async(uuid, new QuoteIN
        {
            projectName = request.ProjectName,
            customerID = request.CustomerId,
            subject = request.ProjectName,
            creationDate = DateTime.Now,
        });
        await authProviders.Logout();
        return new CreateQuoteResponse { QuoteId = quoteIdResult.data};
    }

    private QuoteResponse MapQuoteResponse(Quote? quote)
    {
        return quote == null
            ? new QuoteResponse()
            : new QuoteResponse
            {
                Currency = quote.currency,
                ProjectName = quote.projectName,
                Rate = quote.rate
            };
    }
}