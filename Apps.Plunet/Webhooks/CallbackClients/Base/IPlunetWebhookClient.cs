using Apps.Plunet.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Plunet.Webhooks.CallbackClients.Base;

public interface IPlunetWebhookClient
{
    Task RegisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values, EventType eventType);
    
    Task DeregisterCallback(IEnumerable<AuthenticationCredentialsProvider> creds, EventType eventType);
}