using Apps.Plunet.Webhooks.CallbackClients;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Apps.Plunet.Webhooks.Handlers.Base;
using Apps.Plunet.Webhooks.Models;

namespace Apps.Plunet.Webhooks.Handlers.Impl.Resources;

public class ResourceChangedEventHandler : PlunetWebhookHandler
{
    protected override IPlunetWebhookClient Client => new ResourceClient();
    protected override EventType EventType => EventType.StatusChanged;
}