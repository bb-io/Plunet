using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Orders;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.Utils;

namespace Blackbird.Plugins.Plunet.Webhooks;

[WebhookList]
public class OrderHooks : BaseInvocable
{
    private const string ServiceName = "CallbackOrder30";
    private const string XmlTagName = "OrderID";

    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;
    
    public OrderHooks(InvocationContext invocationContext) : base(invocationContext)
    {
    }
    
    [Webhook("On order deleted", typeof(OrderDeleteEventHandler), Description = "Triggered when an order is deleted")]
    public async Task<WebhookResponse<TriggerContent>> OrderDeleted(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName, Creds);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On order created", typeof(OrderCreatedEventHandler), Description = "Triggered when an order is created")]
    public async Task<WebhookResponse<TriggerContent>> OrderCreated(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName, Creds);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On order status changed", typeof(OrderChangedEventHandler), Description = "Triggered when an order status is changed")]
    public async Task<WebhookResponse<TriggerContent>> OrderStatusChanged(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName, Creds);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

}