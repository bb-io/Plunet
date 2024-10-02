using Apps.Plunet.Webhooks.CallbackClients;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Apps.Plunet.Webhooks.Handlers.Base;
using Apps.Plunet.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.Webhooks.Handlers.Impl.Jobs;

public class JobStartDateChangedEventHandler(InvocationContext invocationContext) : PlunetWebhookHandler(invocationContext)
{
    protected override IPlunetWebhookClient Client => new JobClient();
    protected override EventType EventType => EventType.StartDateChanged;
}