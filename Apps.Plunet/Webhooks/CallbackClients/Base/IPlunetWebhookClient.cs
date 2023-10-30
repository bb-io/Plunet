using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.Webhooks.Models;

namespace Blackbird.Plugins.Plunet.Webhooks.CallbackClients.Base;

public interface IPlunetWebhookClient
{
    Task RegisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values, EventType eventType);
    
    Task DeregisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds, EventType eventType);
}