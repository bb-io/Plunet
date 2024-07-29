﻿using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Apps.Plunet.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Authentication;
using Microsoft.Extensions.Logging;

namespace Apps.Plunet.Webhooks.CallbackClients;

public class OrderClient : IPlunetWebhookClient
{
    public async Task RegisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values, EventType eventType)
    {
        try
        {
            await Logger.LogAsync(new
                { 
                    values, 
                    eventType = eventType.ToString(), 
                    message = "Subscribing callback" 
                });

            var uuid = creds.GetAuthToken();

            await using var orderClient = Clients.GetOrderClient(creds.GetInstanceUrl());
            var result = await orderClient.registerCallback_NotifyAsync(uuid, "bbTestPlugin",
                values[CredsNames.WebhookUrlKey] + "?wsdl",
                (int)eventType);
            
            await Logger.LogAsync(new { result_message = result.statusMessage, status_code = result.statusCode });
            await creds.Logout();
        }
        catch (Exception e)
        {
            await Logger.LogAsync(e);
            throw;
        }
    }

    public async Task DeregisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values,
        EventType eventType)
    {
        try
        {
            await Logger.LogAsync(new { values, eventType = eventType.ToString(), message = "Unsubscribing callback" });
            var uuid = creds.GetAuthToken();

            var dataAdminClient = Clients.GetAdminClient(creds.GetInstanceUrl());

            var callbacks = await dataAdminClient.getListOfRegisteredCallbacksAsync(uuid);
            var eventCallbacks = callbacks.data.Where(c => c.eventType == (int)eventType).ToList();
            
            await Logger.LogAsync(new { eventCallbacks });

            await using var orderClient = Clients.GetOrderClient(creds.GetInstanceUrl());
            var result = await orderClient.deregisterCallback_NotifyAsync(uuid, (int)eventType);

            await Logger.LogAsync(new { result_message = result.statusMessage, status_code = result.statusCode });
            
            foreach(var callback in eventCallbacks.Where(x => x.serverAddress != values[CredsNames.WebhookUrlKey] + "?wsdl"))
            {
                await orderClient.registerCallback_NotifyAsync(uuid, 
                    "bbTestPlugin",
                    callback.serverAddress, 
                    (int)eventType);
            } 
            
            await creds.Logout();
        }
        catch (Exception e)
        {
            await Logger.LogAsync(e);
            throw;
        }
    }
}