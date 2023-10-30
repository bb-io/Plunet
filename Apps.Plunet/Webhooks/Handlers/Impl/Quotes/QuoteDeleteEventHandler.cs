using Blackbird.Plugins.Plunet.Webhooks.CallbackClients;
using Blackbird.Plugins.Plunet.Webhooks.CallbackClients.Base;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Base;
using Blackbird.Plugins.Plunet.Webhooks.Models;

namespace Blackbird.Plugins.Plunet.Webhooks.Handlers.Impl.Quotes;

public class QuoteDeleteEventHandler : PlunetWebhookHandler
{
    protected override IPlunetWebhookClient Client => new QuoteClient();
    protected override EventType EventType => EventType.EntryDeleted;
}