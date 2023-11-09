using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Apps.Plunet.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Plunet.Webhooks.CallbackClients;

public class ItemClient : IPlunetWebhookClient
{
    public async Task RegisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values, EventType eventType)
    {
        var uuid = creds.GetAuthToken();

        await using var orderClient = Clients.GetItemClient(creds.GetInstanceUrl());
        await orderClient.registerCallback_NotifyAsync(uuid, "bbTestPlugin", values[CredsNames.WebhookUrlKey] + "?wsdl",
            (int)eventType);

        await creds.Logout();
    }

    public async Task DeregisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        EventType eventType)
    {
        var uuid = creds.GetAuthToken();

        await using var orderClient = Clients.GetItemClient(creds.GetInstanceUrl());
        await orderClient.deregisterCallback_NotifyAsync(uuid, (int)eventType);

        await creds.Logout();
    }
}