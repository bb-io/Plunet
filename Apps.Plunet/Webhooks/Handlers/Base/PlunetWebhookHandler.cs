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
        var dataAdminClient = Clients.GetAdminClient(creds.GetInstanceUrl());
        var callbacks = await dataAdminClient.getListOfRegisteredCallbacksAsync(uuid);
        var eventCallbacks = callbacks.data.Where(c => c.eventType == (int)EventType).ToList();
        var currentCallback = eventCallbacks.Where(x => x.serverAddress == values[CredsNames.WebhookUrlKey] + "?wsdl").FirstOrDefault();
        var otherCallbacksThatWillBeRemoved = eventCallbacks.Where(x => x.mainID != currentCallback?.mainID && x.dataService == currentCallback?.dataService);

        await Client.DeregisterCallback(creds, values, EventType, uuid);

        foreach (var callback in otherCallbacksThatWillBeRemoved)
        {
            await Client.RegisterCallback(creds, new Dictionary<string, string> { { CredsNames.WebhookUrlKey, callback.serverAddress.Replace("?wsdl", string.Empty) } }, EventType, uuid);
        } 
        
        await creds.Logout();
    }
}