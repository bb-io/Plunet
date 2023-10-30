using Blackbird.Plugins.Plunet.Webhooks.CallbackClients;
using Blackbird.Plugins.Plunet.Webhooks.CallbackClients.Base;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Base;
using Blackbird.Plugins.Plunet.Webhooks.Models;

namespace Blackbird.Plugins.Plunet.Webhooks.Handlers.Impl.Requests;

public class RequestCreatedEventHandler : PlunetWebhookHandler
{
    protected override IPlunetWebhookClient Client => new RequestClient();
    protected override EventType EventType => EventType.NewEntryCreated;
}