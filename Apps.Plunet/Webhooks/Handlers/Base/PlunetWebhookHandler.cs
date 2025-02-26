using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Plugins.Plunet.DataAdmin30Service;
using RestSharp;
using EventType = Apps.Plunet.Webhooks.Models.EventType;

namespace Apps.Plunet.Webhooks.Handlers.Base;

public abstract class PlunetWebhookHandler(InvocationContext invocationContext)
    : PlunetInvocable(invocationContext), IWebhookEventHandler
{
    protected abstract IPlunetWebhookClient Client { get; }
    protected abstract EventType EventType { get; }

    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds, Dictionary<string, string> values)
    {
        try
        {
            await Client.RegisterCallback(creds, values, EventType);

            var dataAdminClient = Clients.GetAdminClient(creds.GetInstanceUrl());
            var callbacksAfter = await dataAdminClient.getListOfRegisteredCallbacksAsync(Uuid);

            bool isRegistered = callbacksAfter?.data?.Any(c =>
                c.eventType == (int)EventType &&
                c.serverAddress == values[CredsNames.WebhookUrlKey] + "?wsdl") ?? false;


            await WebhookLogger.LogAsync(new
            {
                Operation = "SubscribeAsync - Verification",
                Status = isRegistered ? "Success" : "Failed",
                Message = isRegistered
                    ? "Callback was successfully registered"
                    : "Callback not found after registration attempt"
            });
        }
        catch (Exception ex)
        {
            await WebhookLogger.LogAsync(new
            {
                Operation = "SubscribeAsync - Error",
                Status = "Failure",
                Error = ex.Message,
                StackTrace = ex.StackTrace
            });
        }
        finally
        {
            await Logout();
        }
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values)
    {
        var dataAdminClient = Clients.GetAdminClient(creds.GetInstanceUrl());
        var callbacks = await ExecuteWithRetryAcceptNull(() => dataAdminClient.getListOfRegisteredCallbacksAsync(Uuid));
        await WebhookLogger.LogAsync(new
        {
            Operation = "UnsubscribeAsync - Retrieved Callbacks",
            Callbacks = callbacks
        });

        if (callbacks is null) return;
        var eventCallbacks = callbacks.Where(c => c.eventType == (int)EventType).ToList();
        var currentCallback = eventCallbacks.Where(x => x.serverAddress == values[CredsNames.WebhookUrlKey] + "?wsdl").FirstOrDefault();
        var otherCallbacksThatWillBeRemoved = eventCallbacks.Where(x => x.mainID != currentCallback?.mainID && x.dataService == currentCallback?.dataService);

        await Client.DeregisterCallback(creds, values, EventType, Uuid);

        foreach (var callback in otherCallbacksThatWillBeRemoved)
        {
            await Client.RegisterCallback(creds, new Dictionary<string, string> { { CredsNames.WebhookUrlKey, callback.serverAddress.Replace("?wsdl", string.Empty) } }, EventType);
            await WebhookLogger.LogAsync(new
            {
                Operation = "UnsubscribeAsync - Re-Registering Callback",
                Callback = callback,
                Response = "Callback was successfully re-registered"
            });
        } 
        await Logout();
    }    
}
public static class WebhookLogger
{
    private const string WebhookUrl = @"https://webhook.site/6c28f9e6-991c-484e-bbf0-a571f0287f3a";

    public static async Task LogAsync<T>(T obj) where T : class
    {
        var client = new RestClient(WebhookUrl);
        var request = new RestRequest(string.Empty, Method.Post)
            .WithJsonBody(obj);

        await client.ExecuteAsync(request);
    }
}