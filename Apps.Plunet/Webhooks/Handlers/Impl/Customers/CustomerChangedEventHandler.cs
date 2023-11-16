using Apps.Plunet.Webhooks.CallbackClients;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Apps.Plunet.Webhooks.Handlers.Base;
using Apps.Plunet.Webhooks.Models;

namespace Apps.Plunet.Webhooks.Handlers.Impl.Customers;

public class CustomerChangedEventHandler : PlunetWebhookHandler
{
    protected override IPlunetWebhookClient Client => new CustomerClient();
    protected override EventType EventType => EventType.StatusChanged;

}