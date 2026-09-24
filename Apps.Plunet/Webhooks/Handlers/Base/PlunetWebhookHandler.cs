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
    : PlunetInvocable(invocationContext), IWebhookEventHandler, IAsyncValidatableWebhookEventHandler
{
    protected abstract IPlunetWebhookClient Client { get; }
    protected abstract EventType EventType { get; }

    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values)
    {
        await Client.RegisterCallback(creds, values, EventType);
        await Logout();
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values)
    {
        var dataAdminClient = Clients.GetAdminClient(creds.GetInstanceUrl());
        var callbacks = await ExecuteWithRetryAcceptNull(() => dataAdminClient.getListOfRegisteredCallbacksAsync(Uuid));
        if (callbacks is null)
        {
            return;
        }

        var eventCallbacks = callbacks.Where(c => c.eventType == (int)EventType).ToList();
        var currentCallback =
            eventCallbacks.FirstOrDefault(x => x.serverAddress == values[CredsNames.WebhookUrlKey] + "?wsdl");
        if (currentCallback != null)
        {
            var otherCallbacksThatWillBeRemoved = eventCallbacks
                .Where(x => x.mainID != currentCallback?.mainID && x.dataService == currentCallback?.dataService)
                .ToList();

            await Client.DeregisterCallback(creds, values, EventType, Uuid);

            foreach (var callback in otherCallbacksThatWillBeRemoved)
            {
                await Client.RegisterCallback(creds, new Dictionary<string, string>
                {
                    { CredsNames.WebhookUrlKey, callback.serverAddress.Replace("?wsdl", string.Empty) }
                }, EventType);
            }

            await Logout();
        }
    }

    public async Task<WebhookSubscriptionValidationResponse> ValidateSubscription(
        IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values)
    {
        if (!values.TryGetValue(CredsNames.WebhookUrlKey, out var webhookUrl) ||
            string.IsNullOrWhiteSpace(webhookUrl))
        {
            return Invalid("The webhook URL is missing, so the Plunet webhook subscription cannot be validated.");
        }

        try
        {
            var dataAdminClient = Clients.GetAdminClient(creds.GetInstanceUrl());
            var callbacks = await ExecuteWithRetryAcceptNull(() =>
                dataAdminClient.getListOfRegisteredCallbacksAsync(Uuid));

            return IsCallbackRegistered(callbacks, EventType, webhookUrl)
                ? new WebhookSubscriptionValidationResponse { IsValid = true }
                : Invalid("The Plunet webhook subscription no longer exists. Recreate the Bird to subscribe again.");
        }
        catch (Exception exception)
        {
            return Invalid($"Could not verify the Plunet webhook subscription: {exception.Message}");
        }
    }

    internal static bool IsCallbackRegistered(IEnumerable<Callback>? callbacks, EventType eventType,
        string webhookUrl)
    {
        var callbackUrl = webhookUrl + "?wsdl";

        return callbacks?.Any(callback =>
            callback.eventType == (int)eventType &&
            string.Equals(callback.serverAddress, callbackUrl, StringComparison.Ordinal)) == true;
    }

    private static WebhookSubscriptionValidationResponse Invalid(string message)
        => new() { IsValid = false, Message = message };
}
