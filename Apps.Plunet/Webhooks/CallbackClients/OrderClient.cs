using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Apps.Plunet.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Authentication;
using Microsoft.Extensions.Logging;

namespace Apps.Plunet.Webhooks.CallbackClients;

public class OrderClient : IPlunetWebhookClient
{
    public async Task RegisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values, EventType eventType, string? uuid = null)
    {
        uuid ??= creds.GetAuthToken();
        await using var orderClient = Clients.GetOrderClient(creds.GetInstanceUrl());
        await orderClient.registerCallback_NotifyAsync(uuid, "bbTestPlugin",
            values[CredsNames.WebhookUrlKey] + "?wsdl",
            (int)eventType);
    }

    public async Task DeregisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values,
        EventType eventType,
        string uuid)
    {
        await using var orderClient = Clients.GetOrderClient(creds.GetInstanceUrl());
        await orderClient.deregisterCallback_NotifyAsync(uuid, (int)eventType);
    }
}