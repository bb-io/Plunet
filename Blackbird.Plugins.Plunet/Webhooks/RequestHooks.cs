using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Xml.Linq;
using System.Xml.Serialization;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Requests;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.Utils;
using RestSharp;

namespace Blackbird.Plugins.Plunet.Webhooks;

[WebhookList]
public class RequestHooks
{    
    [Webhook("On request deleted", typeof(RequestDeleteEventHandler), Description = "Triggered when a request is deleted")]
    public async Task<WebhookResponse<TriggerContent>> RequestDeleted(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator<RequestCallback>("CallbackRequest30");
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On request created", typeof(RequestCreatedEventHandler), Description = "Triggered when a request is created")]
    public async Task<WebhookResponse<TriggerContent>> RequestCreated(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator<RequestCallback>("CallbackRequest30");
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On request status changed", typeof(RequestChangedEventHandler), Description = "Triggered when a request status is changed")]
    public async Task<WebhookResponse<TriggerContent>> RequestChanged(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator<RequestCallback>("CallbackRequest30");
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }


}