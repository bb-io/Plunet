using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Webhooks.CallbackClients;
using Blackbird.Plugins.Plunet.Webhooks.Utils;

namespace Blackbird.Plugins.Plunet.Webhooks.Handlers.Quotes;

public class QuoteDeleteEventHandler : IWebhookEventHandler
{
    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
    {
        await QuoteClient.RegisterCallback(authenticationCredentialsProviders, values, EventType.ENTRY_DELETED);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
    {
        await QuoteClient.DeregisterCallback(authenticationCredentialsProviders, EventType.ENTRY_DELETED);
    }
}