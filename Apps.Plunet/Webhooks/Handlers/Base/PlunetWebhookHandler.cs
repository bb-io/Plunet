using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.DataAdmin30Service;
using EventType = Apps.Plunet.Webhooks.Models.EventType;

namespace Apps.Plunet.Webhooks.Handlers.Base;

public abstract class PlunetWebhookHandler(InvocationContext invocationContext)
    : PlunetInvocable(invocationContext), IWebhookEventHandler
{
    protected abstract IPlunetWebhookClient Client { get; }
    protected abstract EventType EventType { get; }

    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds, Dictionary<string, string> values)
    {
        await Client.RegisterCallback(creds, values, EventType);
        await Logout();
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values)
    {
        var dataAdminClient = Clients.GetAdminClient(creds.GetInstanceUrl());
        var callbacks = await ExecuteWithRetry<CallbackListResult>(async () => await dataAdminClient.getListOfRegisteredCallbacksAsync(Uuid));
        var eventCallbacks = callbacks.data.Where(c => c.eventType == (int)EventType).ToList();
        var currentCallback = eventCallbacks.Where(x => x.serverAddress == values[CredsNames.WebhookUrlKey] + "?wsdl").FirstOrDefault();
        var otherCallbacksThatWillBeRemoved = eventCallbacks.Where(x => x.mainID != currentCallback?.mainID && x.dataService == currentCallback?.dataService);

        await Client.DeregisterCallback(creds, values, EventType, Uuid);

        foreach (var callback in otherCallbacksThatWillBeRemoved)
        {
            await Client.RegisterCallback(creds, new Dictionary<string, string> { { CredsNames.WebhookUrlKey, callback.serverAddress.Replace("?wsdl", string.Empty) } }, EventType);
        } 
        
        await Logout();
    }
    
    private async Task<T> ExecuteWithRetry<T>(Func<Task<Result>> func, int maxRetries = 10, int delay = 1000)
        where T : Result
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();
            
            if(result.statusMessage == ApiResponses.Ok)
            {
                return (T)result;
            }
            
            if(result.statusMessage.Contains("session-UUID used is invalid"))
            {
                if (attempts < maxRetries)
                {
                    await Task.Delay(delay);
                    await RefreshAuthToken();
                    attempts++;
                    continue;
                }

                throw new($"No more retries left. Last error: {result.statusMessage}, Session UUID used is invalid.");
            }

            return (T)result;
        }
    }
}