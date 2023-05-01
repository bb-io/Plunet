using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Xml.Linq;
using System.Xml.Serialization;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Resources;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.Utils;
using RestSharp;

namespace Blackbird.Plugins.Plunet.Webhooks;

[WebhookList]
public class ResourceHooks
{    
    [Webhook("On resource deleted", typeof(ResourceDeleteEventHandler), Description = "Triggered when a resource is deleted")]
    public async Task<WebhookResponse<TriggerContent>> ResourceDeleted(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator<ResourceCallback>("CallbackResource30");
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On resource created", typeof(ResourceCreatedEventHandler), Description = "Triggered when a resource is created")]
    public async Task<WebhookResponse<TriggerContent>> ResourceCreated(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator<ResourceCallback>("CallbackResource30");
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On resource status changed", typeof(ResourceChangedEventHandler), Description = "Triggered when a resource status is changed")]
    public async Task<WebhookResponse<TriggerContent>> ResourceChanged(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator<ResourceCallback>("CallbackResource30");
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }


}