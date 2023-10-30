using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Impl.Items;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.WebhookLists.Base;

namespace Blackbird.Plugins.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class ItemHooks : PlunetWebhookList
{
    protected override string ServiceName => "CallbackItem30";
    protected override string XmlTagName => "ItemID";

    public ItemHooks(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Webhook("On item deleted", typeof(ItemDeleteEventHandler), Description = "Triggered when an item is deleted")]
    public Task<WebhookResponse<TriggerContent>> ItemDeleted(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On item created", typeof(ItemCreatedEventHandler), Description = "Triggered when an item is created")]
    public Task<WebhookResponse<TriggerContent>> ItemCreated(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On item status changed", typeof(ItemChangedEventHandler),
        Description = "Triggered when an item status is changed")]
    public Task<WebhookResponse<TriggerContent>> ItemStatusChanged(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On item delivery date changed", typeof(ItemDeliveryDateChangedEventHandler),
        Description = "Triggered when an item delivery date is changed")]
    public Task<WebhookResponse<TriggerContent>> ItemDeliveryDateChanged(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);
}