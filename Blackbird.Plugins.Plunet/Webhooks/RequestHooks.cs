using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Requests;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.Utils;

namespace Blackbird.Plugins.Plunet.Webhooks;

[WebhookList]
public class RequestHooks : BaseInvocable
{
    private const string ServiceName = "CallbackRequest30";
    private const string XmlTagName = "RequestID";

    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;
    
    public RequestHooks(InvocationContext invocationContext) : base(invocationContext)
    {
    }
    
    [Webhook("On request deleted", typeof(RequestDeleteEventHandler), Description = "Triggered when a request is deleted")]
    public async Task<WebhookResponse<TriggerContent>> RequestDeleted(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName, Creds);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On request created", typeof(RequestCreatedEventHandler), Description = "Triggered when a request is created")]
    public async Task<WebhookResponse<TriggerContent>> RequestCreated(WebhookRequest webhookRequest)
    {   
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName, Creds);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On request status changed", typeof(RequestChangedEventHandler), Description = "Triggered when a request status is changed")]
    public async Task<WebhookResponse<TriggerContent>> RequestChanged(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName, Creds);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }


}