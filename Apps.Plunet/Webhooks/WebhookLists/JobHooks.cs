using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Impl.Jobs;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.WebhookLists.Base;

namespace Blackbird.Plugins.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class JobHooks : PlunetWebhookList
{
    protected override string ServiceName => "CallbackJob30";
    protected override string XmlTagName => "JobID";

    public JobHooks(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Webhook("On job deleted", typeof(JobDeleteEventHandler), Description = "Triggered when a job is deleted")]
    public Task<WebhookResponse<TriggerContent>> JobDeleted(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On job created", typeof(JobCreatedEventHandler), Description = "Triggered when a job is created")]
    public Task<WebhookResponse<TriggerContent>> JobCreated(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On job status changed", typeof(JobChangedEventHandler),
        Description = "Triggered when a job status is changed")]
    public Task<WebhookResponse<TriggerContent>> JobStatusChanged(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On job delivery date changed", typeof(JobDeliveryDateChangedEventHandler),
        Description = "Triggered when a job delivery date is changed")]
    public Task<WebhookResponse<TriggerContent>> JobDeliveryDateChanged(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On job start date changed", typeof(JobStartDateChangedEventHandler),
        Description = "Triggered when a job start date is changed")]
    public Task<WebhookResponse<TriggerContent>> JobStartDateChanged(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);
}