﻿using Apps.Plunet.Api;
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
        await ExecuteWithRetry<Result>(async () => await orderClient.registerCallback_NotifyAsync(Uuid, "bbTestPlugin",
            values[CredsNames.WebhookUrlKey] + "?wsdl",
            (int)eventType));
    }

    public async Task DeregisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values,
        EventType eventType,
        string uuid)
    {
        await using var orderClient = Clients.GetOrderClient(creds.GetInstanceUrl());
        await ExecuteWithRetry<Result>(async () =>
            await orderClient.deregisterCallback_NotifyAsync(Uuid, (int)eventType));
    }
    
    private async Task<T> ExecuteWithRetry<T>(Func<Task<Result>> func, int maxRetries = 10, int delay = 1000)
        where T : Result
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();

            if (result.statusMessage == ApiResponses.Ok)
            {
                return (T)result;
            }

            if (result.statusMessage.Contains("session-UUID used is invalid"))
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