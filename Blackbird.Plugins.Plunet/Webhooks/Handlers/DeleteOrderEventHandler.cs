using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Connections;
using Blackbird.Plugins.Plunet.DataOrder30Service;

namespace Blackbird.Plugins.Plunet.Webhooks.Handlers;

public class DeleteOrderEventHandler : IWebhookEventHandler
{
    private const string ApiKeyName = "UUID";


    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
    {
        var uuid = authenticationCredentialsProviders.FirstOrDefault(x => x.KeyName == ApiKeyName)?.Value;
        using var orderClient = new DataOrder30Client();
        await orderClient.registerCallback_NotifyAsync(uuid, "bbTestPlugin", values["payloadUrl"]+"?wsdl", 3);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
    {
        var uuid = authenticationCredentialsProviders.FirstOrDefault(x => x.KeyName == ApiKeyName)?.Value;
        using var orderClient = new DataOrder30Client();
        await orderClient.deregisterCallback_NotifyAsync(uuid, 3);
    }
}