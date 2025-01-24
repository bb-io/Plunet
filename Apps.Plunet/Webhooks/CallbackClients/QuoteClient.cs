using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Apps.Plunet.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataQuote30Service;

namespace Apps.Plunet.Webhooks.CallbackClients;

public class QuoteClient(InvocationContext invocationContext) : PlunetInvocable(invocationContext), IPlunetWebhookClient
{
    public async Task RegisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values, EventType eventType, string? uuid = null)
    {
        await using var quoteClient = Clients.GetQuoteClient(creds.GetInstanceUrl());
        await ExecuteWithRetry(() => quoteClient.registerCallback_NotifyAsync(Uuid, "bbTestPlugin",
            values[CredsNames.WebhookUrlKey] + "?wsdl",
            (int)eventType));

        await Logout();
    }

    public async Task DeregisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values,
        EventType eventType,
        string uuid)
    {
        await using var quoteClient = Clients.GetQuoteClient(creds.GetInstanceUrl());
        await ExecuteWithRetry(() => quoteClient.deregisterCallback_NotifyAsync(Uuid, (int)eventType));

        await Logout();
    }
    
}