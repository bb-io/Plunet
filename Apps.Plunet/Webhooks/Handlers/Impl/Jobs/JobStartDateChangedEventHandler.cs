using Blackbird.Plugins.Plunet.Webhooks.CallbackClients;
using Blackbird.Plugins.Plunet.Webhooks.CallbackClients.Base;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Base;
using Blackbird.Plugins.Plunet.Webhooks.Models;

namespace Blackbird.Plugins.Plunet.Webhooks.Handlers.Impl.Jobs;

public class JobStartDateChangedEventHandler : PlunetWebhookHandler
{
    protected override IPlunetWebhookClient Client => new JobClient();
    protected override EventType EventType => EventType.StartDateChanged;
}