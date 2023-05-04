using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Xml.Linq;
using System.Xml.Serialization;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Orders;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.Utils;
using RestSharp;

namespace Blackbird.Plugins.Plunet.Webhooks;

[WebhookList]
public class OrderHooks
{
    private const string ServiceName = "CallbackOrder30";
    private const string XmlTagName = "OrderID";

    [Webhook("On order deleted", typeof(OrderDeleteEventHandler), Description = "Triggered when an order is deleted")]
    public async Task<WebhookResponse<TriggerContent>> OrderDeleted(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On order created", typeof(OrderCreatedEventHandler), Description = "Triggered when an order is created")]
    public async Task<WebhookResponse<TriggerContent>> OrderCreated(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On order status changed", typeof(OrderChangedEventHandler), Description = "Triggered when an order status is changed")]
    public async Task<WebhookResponse<TriggerContent>> OrderStatusChanged(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

}