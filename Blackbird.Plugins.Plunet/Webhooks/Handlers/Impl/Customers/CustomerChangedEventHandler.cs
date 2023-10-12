using Blackbird.Plugins.Plunet.Webhooks.CallbackClients;
using Blackbird.Plugins.Plunet.Webhooks.CallbackClients.Base;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Base;
using Blackbird.Plugins.Plunet.Webhooks.Models;

namespace Blackbird.Plugins.Plunet.Webhooks.Handlers.Impl.Customers;

public class CustomerChangedEventHandler : PlunetWebhookHandler
{
    protected override IPlunetWebhookClient Client => new CustomerClient();
    protected override EventType EventType => EventType.StatusChanged;

}