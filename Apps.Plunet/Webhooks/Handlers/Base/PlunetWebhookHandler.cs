using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using EventType = Apps.Plunet.Webhooks.Models.EventType;

namespace Apps.Plunet.Webhooks.Handlers.Base;

public abstract class PlunetWebhookHandler(InvocationContext invocationContext)
    : PlunetInvocable(invocationContext), IWebhookEventHandler
{
    protected abstract IPlunetWebhookClient Client { get; }
    protected abstract EventType EventType { get; }

    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values)
    {
        invocationContext.Logger?.LogInformation("[Plunet app] Called SubscribeAsync", ["values"]);

        await Client.RegisterCallback(creds, values, EventType);
        await Logout();

        invocationContext.Logger?.LogInformation("[Plunet app] Successfully Subscribed", ["values"]);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values)
    {
        try
        {
            invocationContext.Logger?.LogInformation("[Plunet app] Called UnsubscribeAsync", ["values"]);

            var dataAdminClient = Clients.GetAdminClient(creds.GetInstanceUrl());
            var callbacks =
                await ExecuteWithRetryAcceptNull(() => dataAdminClient.getListOfRegisteredCallbacksAsync(Uuid));
            if (callbacks is null)
            {
                return;
            }

            var eventCallbacks = callbacks.Where(c => c.eventType == (int)EventType).ToList();
            var currentCallback = eventCallbacks.FirstOrDefault(x => x.serverAddress == values[CredsNames.WebhookUrlKey] + "?wsdl");
            
            if (currentCallback != null)
            {
                invocationContext.Logger?.LogInformation(
                    $"[Plunet app] Determined currentCallback: {currentCallback.serverAddress}",
                    [currentCallback.serverAddress]);

                var otherCallbacksThatWillBeRemoved = eventCallbacks
                    .Where(x => x.mainID != currentCallback?.mainID && x.dataService == currentCallback?.dataService)
                    .ToList();

                var otherCallbacks =
                    string.Join(", ", otherCallbacksThatWillBeRemoved.Select(x => x.serverAddress).ToList());

                invocationContext.Logger?.LogInformation(
                    $"[Plunet app] Determined otherCallbacksThatWillBeRemoved: {otherCallbacks}",
                    [otherCallbacks]);
            
                await Client.DeregisterCallback(creds, values, EventType, Uuid);

                foreach (var callback in otherCallbacksThatWillBeRemoved)
                {
                    await Client.RegisterCallback(creds, new Dictionary<string, string>
                    {
                        { CredsNames.WebhookUrlKey, callback.serverAddress.Replace("?wsdl", string.Empty) }
                    }, EventType);
                }

                await Logout();
            
                invocationContext.Logger?.LogInformation(
                    $"[Plunet app] Successfully UnsubscribeAsync",
                    ["test"]);   
            }
        }
        catch (Exception e)
        {
            invocationContext.Logger?.LogError(
                $"[Plunet app] Received exception in UnsubscribeAsync method {e.Message}",
                [e.Message]);
            throw;
        }
    }
}