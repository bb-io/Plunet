using Apps.Plunet.Webhooks.CallbackClients;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Apps.Plunet.Webhooks.Handlers.Base;
using Apps.Plunet.Webhooks.Models;

namespace Apps.Plunet.Webhooks.Handlers.Impl.Requests;

public class RequestChangedEventHandler : PlunetWebhookHandler
{
    protected override IPlunetWebhookClient Client => new RequestClient();
    protected override EventType EventType => EventType.StatusChanged;
}