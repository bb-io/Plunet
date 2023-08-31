using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Customers;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.Utils;

namespace Blackbird.Plugins.Plunet.Webhooks;

[WebhookList]
public class CustomerHooks : BaseInvocable
{
    private const string ServiceName = "CallbackCustomer30";
    private const string XmlTagName = "CustomerID";

    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;
    
    public CustomerHooks(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Webhook("On customer deleted", typeof(CustomerDeleteEventHandler), Description = "Triggered when a customer is deleted")]
    public async Task<WebhookResponse<TriggerContent>> CustomerDeleted(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName, Creds);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On customer created", typeof(CustomerCreatedEventHandler), Description = "Triggered when a customer is created")]
    public async Task<WebhookResponse<TriggerContent>> CustomerCreated(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName, Creds);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On customer status changed", typeof(CustomerChangedEventHandler), Description = "Triggered when a customer status is changed")]
    public async Task<WebhookResponse<TriggerContent>> CustomerChanged(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName, Creds);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }
}