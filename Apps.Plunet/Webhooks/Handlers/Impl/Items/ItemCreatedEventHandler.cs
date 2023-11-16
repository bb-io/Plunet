using Apps.Plunet.Webhooks.CallbackClients;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Apps.Plunet.Webhooks.Handlers.Base;
using Apps.Plunet.Webhooks.Models;

namespace Apps.Plunet.Webhooks.Handlers.Impl.Items;

public class ItemCreatedEventHandler : PlunetWebhookHandler
{
    protected override IPlunetWebhookClient Client => new ItemClient();
    protected override EventType EventType => EventType.NewEntryCreated;
}