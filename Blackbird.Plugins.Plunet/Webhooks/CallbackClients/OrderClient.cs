﻿using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Webhooks.Utils;

namespace Blackbird.Plugins.Plunet.Webhooks.CallbackClients
{
    public static class OrderClient
    {
        //ToDo: remove magic dictionary keys
        private const string WebhookUrlKey = "payloadUrl";

        public static async Task RegisterCallback(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values, EventType eventType)
        {
            var uuid = authenticationCredentialsProviders.GetAuthToken();
            await using var orderClient = Clients.GetOrderClient(authenticationCredentialsProviders.GetInstanceUrl());
            await orderClient.registerCallback_NotifyAsync(uuid, "bbTestPlugin", values[WebhookUrlKey] + "?wsdl", (int)eventType);
            await authenticationCredentialsProviders.Logout();
        }

        public static async Task DeregisterCallback(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, EventType eventType)
        {
            var uuid = authenticationCredentialsProviders.GetAuthToken();
            await using var orderClient = Clients.GetOrderClient(authenticationCredentialsProviders.GetInstanceUrl());
            await orderClient.deregisterCallback_NotifyAsync(uuid, (int)eventType);
            await authenticationCredentialsProviders.Logout();
        }
    }
}
