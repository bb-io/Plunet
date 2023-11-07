﻿using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.CallbackClients.Base;
using Blackbird.Plugins.Plunet.Webhooks.Models;

namespace Blackbird.Plugins.Plunet.Webhooks.Handlers.Base;

public abstract class PlunetWebhookHandler : IWebhookEventHandler
{
    protected abstract IPlunetWebhookClient Client { get; }
    protected abstract EventType EventType { get; }

    public Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds, Dictionary<string, string> values)
        => Client.RegisterCallback(creds, values, EventType);

    public Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values)
        => Client.DeregisterCallback(creds, EventType);
}