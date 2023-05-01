using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Xml.Linq;
using System.Xml.Serialization;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Customers;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.Utils;
using RestSharp;

namespace Blackbird.Plugins.Plunet.Webhooks;

[WebhookList]
public class CustomerHooks
{    
    [Webhook("On customer deleted", typeof(CustomerDeleteEventHandler), Description = "Triggered when a customer is deleted")]
    public async Task<WebhookResponse<TriggerContent>> CustomerDeleted(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator<CustomerCallback>("CallbackCustomer30");
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On customer created", typeof(CustomerCreatedEventHandler), Description = "Triggered when a customer is created")]
    public async Task<WebhookResponse<TriggerContent>> CustomerCreated(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator<CustomerCallback>("CallbackCustomer30");
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On customer status changed", typeof(CustomerChangedEventHandler), Description = "Triggered when a customer status is changed")]
    public async Task<WebhookResponse<TriggerContent>> CustomerChanged(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator<CustomerCallback>("CallbackCustomer30");
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }


}