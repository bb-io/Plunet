﻿using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Webhooks.CallbackClients;
using Blackbird.Plugins.Plunet.Webhooks.Utils;

namespace Blackbird.Plugins.Plunet.Webhooks.Handlers.Items;

public class ItemDeliveryDateChangedEventHandler : IWebhookEventHandler
{
    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
    {
        await ItemClient.RegisterCallback(authenticationCredentialsProviders, values, EventType.DELIVERY_DATE_CHANGED);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
    {
        await ItemClient.DeregisterCallback(authenticationCredentialsProviders, EventType.DELIVERY_DATE_CHANGED);
    }
}