using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Webhooks.Utils;

namespace Blackbird.Plugins.Plunet.Webhooks.CallbackClients;

public static class JobClient
{
    //ToDo: remove magic dictionary keys
    private const string WebhookUrlKey = "payloadUrl";

    public static async Task RegisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds, Dictionary<string, string> values, EventType eventType)
    {
        var uuid = creds.GetAuthToken();
        await using var orderClient = Clients.GetJobClient(creds.GetInstanceUrl());
        await orderClient.registerCallback_NotifyAsync(uuid, "bbTestPlugin", values[WebhookUrlKey] + "?wsdl", (int)eventType);
        await creds.Logout();
    }

    public static async Task DeregisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds, EventType eventType)
    {
        var uuid = creds.GetAuthToken();
        await using var orderClient = Clients.GetJobClient(creds.GetInstanceUrl());
        await orderClient.deregisterCallback_NotifyAsync(uuid, (int)eventType);
        await creds.Logout();
    }
}