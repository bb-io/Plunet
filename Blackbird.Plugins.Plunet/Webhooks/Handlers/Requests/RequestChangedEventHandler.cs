﻿using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.CallbackClients;
using Blackbird.Plugins.Plunet.Webhooks.Utils;

namespace Blackbird.Plugins.Plunet.Webhooks.Handlers.Requests;

public class RequestChangedEventHandler : IWebhookEventHandler
{
    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds, Dictionary<string, string> values)
    {
        await RequestClient.RegisterCallback(creds, values, EventType.StatusChanged);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds, Dictionary<string, string> values)
    {
        await RequestClient.DeregisterCallback(creds, EventType.StatusChanged);
    }
}