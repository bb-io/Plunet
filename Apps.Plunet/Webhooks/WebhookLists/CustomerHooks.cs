﻿using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Impl.Customers;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.WebhookLists.Base;

namespace Blackbird.Plugins.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class CustomerHooks : PlunetWebhookList
{
    protected override string ServiceName => "CallbackCustomer30";
    protected override string XmlTagName => "CustomerID";

    public CustomerHooks(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Webhook("On customer deleted", typeof(CustomerDeleteEventHandler),
        Description = "Triggered when a customer is deleted")]
    public Task<WebhookResponse<TriggerContent>> CustomerDeleted(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On customer created", typeof(CustomerCreatedEventHandler),
        Description = "Triggered when a customer is created")]
    public Task<WebhookResponse<TriggerContent>> CustomerCreated(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);

    [Webhook("On customer status changed", typeof(CustomerChangedEventHandler),
        Description = "Triggered when a customer status is changed")]
    public Task<WebhookResponse<TriggerContent>> CustomerChanged(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest);
}