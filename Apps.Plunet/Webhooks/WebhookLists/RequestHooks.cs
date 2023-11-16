using Apps.Plunet.Webhooks.Handlers.Impl.Requests;
using Apps.Plunet.Webhooks.Models;
using Apps.Plunet.Webhooks.WebhookLists.Base;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Plunet.Webhooks.WebhookLists;

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