using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Xml.Linq;
using System.Xml.Serialization;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Items;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.Utils;
using RestSharp;

namespace Blackbird.Plugins.Plunet.Webhooks;

[WebhookList]
public class ItemHooks
{
    private const string ServiceName = "CallbackItem30";
    private const string XmlTagName = "ItemID";

    [Webhook("On item deleted", typeof(ItemDeleteEventHandler), Description = "Triggered when an item is deleted")]
    public async Task<WebhookResponse<TriggerContent>> ItemDeleted(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On item created", typeof(ItemCreatedEventHandler), Description = "Triggered when an item is created")]
    public async Task<WebhookResponse<TriggerContent>> ItemCreated(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On item status changed", typeof(ItemChangedEventHandler), Description = "Triggered when an item status is changed")]
    public async Task<WebhookResponse<TriggerContent>> ItemStatusChanged(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On item delivery date changed", typeof(ItemDeliveryDateChangedEventHandler), Description = "Triggered when an item delivery date is changed")]
    public async Task<WebhookResponse<TriggerContent>> ItemDeliveryDateChanged(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

}