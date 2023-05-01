using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Xml.Linq;
using System.Xml.Serialization;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Items;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Jobs;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.Utils;
using RestSharp;

namespace Blackbird.Plugins.Plunet.Webhooks;

[WebhookList]
public class JobHooks
{    
    [Webhook("On job deleted", typeof(JobDeleteEventHandler), Description = "Triggered when a job is deleted")]
    public async Task<WebhookResponse<TriggerContent>> JobDeleted(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator<JobCallback>("CallbackJob30");
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On job created", typeof(JobCreatedEventHandler), Description = "Triggered when a job is created")]
    public async Task<WebhookResponse<TriggerContent>> JobCreated(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator<JobCallback>("CallbackJob30");
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On job status changed", typeof(JobChangedEventHandler), Description = "Triggered when a job status is changed")]
    public async Task<WebhookResponse<TriggerContent>> JobStatusChanged(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator<JobCallback>("CallbackJob30");
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On job delivery date changed", typeof(JobDeliveryDateChangedEventHandler), Description = "Triggered when a job delivery date is changed")]
    public async Task<WebhookResponse<TriggerContent>> JobDeliveryDateChanged(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator<JobCallback>("CallbackJob30");
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On job start date changed", typeof(JobStartDateChangedEventHandler), Description = "Triggered when a job start date is changed")]
    public async Task<WebhookResponse<TriggerContent>> JobStartDateChanged(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator<JobCallback>("CallbackJob30");
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

}