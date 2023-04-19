using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Extensions;

namespace Blackbird.Plugins.Plunet.Webhooks.Handlers;

public class DeleteOrderEventHandler : IWebhookEventHandler
{
    private const string ApiKeyName = "UUID";
    //ToDo: remove magic dictionary keys
    private const string WebhookUrlKey = "payloadUrl";

    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
    {
        var uuid = authenticationCredentialsProviders.GetAuthToken();
        using var orderClient = Clients.GetOrderClient(authenticationCredentialsProviders.GetInstanceUrl());
        await orderClient.registerCallback_NotifyAsync(uuid, "bbTestPlugin", values[WebhookUrlKey]+"?wsdl", 3);
        await authenticationCredentialsProviders.Logout();
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
    {
        var uuid = authenticationCredentialsProviders.GetAuthToken();
        using var orderClient = Clients.GetOrderClient(authenticationCredentialsProviders.GetInstanceUrl());
        await orderClient.deregisterCallback_NotifyAsync(uuid, 3);
        await authenticationCredentialsProviders.Logout();
    }
}