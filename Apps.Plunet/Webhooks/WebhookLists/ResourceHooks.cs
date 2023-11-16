using Apps.Plunet.Webhooks.Handlers.Impl.Resources;
using Apps.Plunet.Webhooks.Models;
using Apps.Plunet.Webhooks.WebhookLists.Base;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class ResourceHooks : PlunetWebhookList
{
    protected override string ServiceName => "CallbackResource30";
    protected override string XmlTagName => "ResourceID";

    public ResourceHooks(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Webhook("On resource deleted", typeof(ResourceDeleteEventHandler),
        Description = "Triggered when a resource is deleted")]
    public Task<WebhookResponse<TriggerContent>> ResourceDeleted(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On resource created", typeof(ResourceCreatedEventHandler),
        Description = "Triggered when a resource is created")]
    public Task<WebhookResponse<TriggerContent>> ResourceCreated(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On resource status changed", typeof(ResourceChangedEventHandler),
        Description = "Triggered when a resource status is changed")]
    public Task<WebhookResponse<TriggerContent>> ResourceChanged(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);
}