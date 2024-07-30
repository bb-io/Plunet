using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using EventType = Apps.Plunet.Webhooks.Models.EventType;

namespace Apps.Plunet.Webhooks.Handlers.Base;

public abstract class PlunetWebhookHandler : IWebhookEventHandler
{
    protected abstract IPlunetWebhookClient Client { get; }
    protected abstract EventType EventType { get; }

    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds, Dictionary<string, string> values)
    {
        await Client.RegisterCallback(creds, values, EventType);
        await creds.Logout();
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values)
    {
        var uuid = creds.GetAuthToken();
        await Client.DeregisterCallback(creds, values, EventType, uuid);
        
        var dataAdminClient = Clients.GetAdminClient(creds.GetInstanceUrl());
        var callbacks = await dataAdminClient.getListOfRegisteredCallbacksAsync(uuid);
        var eventCallbacks = callbacks.data.Where(c => c.eventType == (int)EventType).ToList();
        
        await Logger.LogAsync(new { eventCallbacks });
        foreach(var callback in eventCallbacks.Where(x => x.serverAddress != values[CredsNames.WebhookUrlKey] + "?wsdl"))
        {
            values[CredsNames.WebhookUrlKey] = callback.serverAddress.Replace("?wsdl", string.Empty);
            await Client.RegisterCallback(creds, values, EventType, uuid);
        } 
        
        await creds.Logout();
    }
}