using Apps.Plunet.Webhooks.CallbackClients;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Apps.Plunet.Webhooks.Handlers.Base;
using Apps.Plunet.Webhooks.Models;

namespace Apps.Plunet.Webhooks.Handlers.Impl.Jobs;

public class JobChangedEventHandler : PlunetWebhookHandler
{
    protected override IPlunetWebhookClient Client => new JobClient();
    protected override EventType EventType => EventType.StatusChanged;
}