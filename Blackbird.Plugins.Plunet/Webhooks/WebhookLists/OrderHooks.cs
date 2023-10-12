﻿using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Impl.Orders;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.WebhookLists.Base;

namespace Blackbird.Plugins.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class OrderHooks : PlunetWebhookList
{
    protected override string ServiceName => "CallbackOrder30";
    protected override string XmlTagName => "OrderID";

    public OrderHooks(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Webhook("On order deleted", typeof(OrderDeleteEventHandler), Description = "Triggered when an order is deleted")]
    public Task<WebhookResponse<TriggerContent>> OrderDeleted(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On order created", typeof(OrderCreatedEventHandler), Description = "Triggered when an order is created")]
    public Task<WebhookResponse<TriggerContent>> OrderCreated(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On order status changed", typeof(OrderChangedEventHandler),
        Description = "Triggered when an order status is changed")]
    public Task<WebhookResponse<TriggerContent>> OrderStatusChanged(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);
}