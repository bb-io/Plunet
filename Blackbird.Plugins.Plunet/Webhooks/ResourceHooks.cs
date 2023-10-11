using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Resources;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.Utils;

namespace Blackbird.Plugins.Plunet.Webhooks;

[WebhookList]
public class ResourceHooks : BaseInvocable
{
    private const string ServiceName = "CallbackResource30";
    private const string XmlTagName = "ResourceID";

    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;

    public ResourceHooks(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Webhook("On resource deleted", typeof(ResourceDeleteEventHandler),
        Description = "Triggered when a resource is deleted")]
    public async Task<WebhookResponse<TriggerContent>> ResourceDeleted(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName, Creds);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On resource created", typeof(ResourceCreatedEventHandler),
        Description = "Triggered when a resource is created")]
    public async Task<WebhookResponse<TriggerContent>> ResourceCreated(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName, Creds);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On resource status changed", typeof(ResourceChangedEventHandler),
        Description = "Triggered when a resource status is changed")]
    public async Task<WebhookResponse<TriggerContent>> ResourceChanged(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName, Creds);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }
}