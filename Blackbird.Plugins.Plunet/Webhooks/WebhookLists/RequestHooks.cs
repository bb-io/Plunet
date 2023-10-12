using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Impl.Requests;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.WebhookLists.Base;

namespace Blackbird.Plugins.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class RequestHooks : PlunetWebhookList
{
    protected override string ServiceName => "CallbackRequest30";
    protected override string XmlTagName => "RequestID";

    public RequestHooks(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Webhook("On request deleted", typeof(RequestDeleteEventHandler),
        Description = "Triggered when a request is deleted")]
    public Task<WebhookResponse<TriggerContent>> RequestDeleted(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On request created", typeof(RequestCreatedEventHandler),
        Description = "Triggered when a request is created")]
    public Task<WebhookResponse<TriggerContent>> RequestCreated(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On request status changed", typeof(RequestChangedEventHandler),
        Description = "Triggered when a request status is changed")]
    public Task<WebhookResponse<TriggerContent>> RequestChanged(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);
}