using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Apps.Plunet.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Plunet.Webhooks.CallbackClients;

public class OrderClient : IPlunetWebhookClient
{
    public async Task RegisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values, EventType eventType)
    {
        try
        {
            await Logger.LogAsync(new
                { creds, values, eventType = eventType.ToString(), message = "Registering callback" });

            var uuid = creds.GetAuthToken();

            await using var orderClient = Clients.GetOrderClient(creds.GetInstanceUrl());
            await orderClient.registerCallback_NotifyAsync(uuid, "bbTestPlugin",
                values[CredsNames.WebhookUrlKey] + "?wsdl",
                (int)eventType);

            await creds.Logout();
        }
        catch (Exception e)
        {
            await Logger.LogAsync(e);
            throw;
        }
    }

    public async Task DeregisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        EventType eventType)
    {
        try
        {
            await Logger.LogAsync(new { creds, eventType = eventType.ToString(), message = "Deregistering callback" });
            var uuid = creds.GetAuthToken();

            await using var orderClient = Clients.GetOrderClient(creds.GetInstanceUrl());
            await orderClient.deregisterCallback_NotifyAsync(uuid, (int)eventType);

            await creds.Logout();
        }
        catch (Exception e)
        {
            await Logger.LogAsync(e);
            throw;
        }
    }
}