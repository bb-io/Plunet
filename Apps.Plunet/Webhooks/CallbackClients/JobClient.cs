using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Apps.Plunet.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataJob30Service;

namespace Apps.Plunet.Webhooks.CallbackClients;

public class JobClient(InvocationContext invocationContext) : PlunetInvocable(invocationContext), IPlunetWebhookClient
{
    public async Task RegisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values, EventType eventType, string? uuid = null)
    {
        await using var jobClient = Clients.GetJobClient(creds.GetInstanceUrl());
        await ExecuteWithRetry(() => jobClient.registerCallback_NotifyAsync(Uuid, "bbTestPlugin",
            values[CredsNames.WebhookUrlKey] + "?wsdl",
            (int)eventType));
    }

    public async Task DeregisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values,
        EventType eventType,
        string uuid)
    {
        await using var jobClient = Clients.GetJobClient(creds.GetInstanceUrl());
        await ExecuteWithRetry(() => jobClient.deregisterCallback_NotifyAsync(Uuid, (int)eventType));
    }    
}