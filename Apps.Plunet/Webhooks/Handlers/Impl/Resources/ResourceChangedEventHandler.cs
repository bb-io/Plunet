using Apps.Plunet.Webhooks.CallbackClients;
using Apps.Plunet.Webhooks.CallbackClients.Base;
using Apps.Plunet.Webhooks.Handlers.Base;
using Apps.Plunet.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.Webhooks.Handlers.Impl.Resources;

public class ResourceChangedEventHandler(InvocationContext invocationContext) : PlunetWebhookHandler(invocationContext)
{
    protected override IPlunetWebhookClient Client => new ResourceClient();
    protected override EventType EventType => EventType.StatusChanged;
}