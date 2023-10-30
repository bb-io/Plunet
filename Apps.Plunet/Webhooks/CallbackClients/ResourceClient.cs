using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.Api;
using Blackbird.Plugins.Plunet.Constants;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Webhooks.CallbackClients.Base;
using Blackbird.Plugins.Plunet.Webhooks.Models;

namespace Blackbird.Plugins.Plunet.Webhooks.CallbackClients;

public class ResourceClient : IPlunetWebhookClient
{
    public async Task RegisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values, EventType eventType)
    {
        var uuid = creds.GetAuthToken();

        await using var orderClient = Clients.GetResourceClient(creds.GetInstanceUrl());
        await orderClient.registerCallback_NotifyAsync(uuid, "bbTestPlugin", values[CredsNames.WebhookUrlKey] + "?wsdl",
            (int)eventType);

        await creds.Logout();
    }

    public async Task DeregisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        EventType eventType)
    {
        var uuid = creds.GetAuthToken();

        await using var orderClient = Clients.GetResourceClient(creds.GetInstanceUrl());
        await orderClient.deregisterCallback_NotifyAsync(uuid, (int)eventType);

        await creds.Logout();
    }
}