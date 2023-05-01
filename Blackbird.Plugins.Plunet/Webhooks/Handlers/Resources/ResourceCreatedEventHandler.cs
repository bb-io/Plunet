using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Webhooks.CallbackClients;
using Blackbird.Plugins.Plunet.Webhooks.Utils;

namespace Blackbird.Plugins.Plunet.Webhooks.Handlers.Resources;

public class ResourceCreatedEventHandler : IWebhookEventHandler
{
    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
    {
        await ResourceClient.RegisterCallback(authenticationCredentialsProviders, values, EventType.NEW_ENTRY_CREATED);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
    {
        await ResourceClient.DeregisterCallback(authenticationCredentialsProviders, EventType.NEW_ENTRY_CREATED);
    }
}