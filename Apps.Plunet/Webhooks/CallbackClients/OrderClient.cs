using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Apps.Plunet.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataOrder30Service;

namespace Apps.Plunet.Webhooks.CallbackClients;

public class OrderClient(InvocationContext invocationContext) : PlunetInvocable(invocationContext), IPlunetWebhookClient
{
    public async Task RegisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values, EventType eventType, string? uuid = null)
    {
        await using var orderClient = Clients.GetOrderClient(creds.GetInstanceUrl());
        await ExecuteWithRetry(() => orderClient.registerCallback_NotifyAsync(Uuid, "bbTestPlugin",
            values[CredsNames.WebhookUrlKey] + "?wsdl",
            (int)eventType));
    }

    public async Task DeregisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values,
        EventType eventType,
        string uuid)
    {
        await using var orderClient = Clients.GetOrderClient(creds.GetInstanceUrl());
        await ExecuteWithRetry(() => orderClient.deregisterCallback_NotifyAsync(Uuid, (int)eventType));
    }    
}