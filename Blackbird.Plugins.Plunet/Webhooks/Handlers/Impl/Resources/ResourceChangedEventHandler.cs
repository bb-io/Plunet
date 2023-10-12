using Blackbird.Plugins.Plunet.Webhooks.CallbackClients;
using Blackbird.Plugins.Plunet.Webhooks.CallbackClients.Base;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Base;
using Blackbird.Plugins.Plunet.Webhooks.Models;

namespace Blackbird.Plugins.Plunet.Webhooks.Handlers.Impl.Resources;

public class ResourceChangedEventHandler : PlunetWebhookHandler
{
    protected override IPlunetWebhookClient Client => new ResourceClient();
    protected override EventType EventType => EventType.StatusChanged;
}