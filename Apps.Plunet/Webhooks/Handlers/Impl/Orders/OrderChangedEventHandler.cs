using Apps.Plunet.Webhooks.CallbackClients;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Apps.Plunet.Webhooks.Handlers.Base;
using Apps.Plunet.Webhooks.Models;

namespace Apps.Plunet.Webhooks.Handlers.Impl.Orders;

public class OrderChangedEventHandler : PlunetWebhookHandler
{
    protected override IPlunetWebhookClient Client => new OrderClient();
    protected override EventType EventType => EventType.StatusChanged;
}