using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Xml.Linq;
using System.Xml.Serialization;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Quotes;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.Utils;
using RestSharp;

namespace Blackbird.Plugins.Plunet.Webhooks;

[WebhookList]
public class QuoteHooks
{
    private const string ServiceName = "CallbackQuote30";
    private const string XmlTagName = "QuoteID";

    [Webhook("On quote deleted", typeof(QuoteDeleteEventHandler), Description = "Triggered when a quote is deleted")]
    public async Task<WebhookResponse<TriggerContent>> QuoteDeleted(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On quote created", typeof(QuoteCreatedEventHandler), Description = "Triggered when a quote is created")]
    public async Task<WebhookResponse<TriggerContent>> QuoteCreated(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On quote status changed", typeof(QuoteChangedEventHandler), Description = "Triggered when a quote status is changed")]
    public async Task<WebhookResponse<TriggerContent>> QuoteStatusChanged(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

}