using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Impl.Quotes;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.WebhookLists.Base;

namespace Blackbird.Plugins.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class QuoteHooks : PlunetWebhookList
{
    protected override string ServiceName => "CallbackQuote30";
    protected override string XmlTagName => "QuoteID";

    public QuoteHooks(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Webhook("On quote deleted", typeof(QuoteDeleteEventHandler), Description = "Triggered when a quote is deleted")]
    public Task<WebhookResponse<TriggerContent>> QuoteDeleted(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On quote created", typeof(QuoteCreatedEventHandler), Description = "Triggered when a quote is created")]
    public Task<WebhookResponse<TriggerContent>> QuoteCreated(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On quote status changed", typeof(QuoteChangedEventHandler),
        Description = "Triggered when a quote status is changed")]
    public Task<WebhookResponse<TriggerContent>> QuoteStatusChanged(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);
}