﻿using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Quotes;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using Blackbird.Plugins.Plunet.Webhooks.Utils;

namespace Blackbird.Plugins.Plunet.Webhooks;

[WebhookList]
public class QuoteHooks : BaseInvocable
{
    private const string ServiceName = "CallbackQuote30";
    private const string XmlTagName = "QuoteID";

    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;
    
    public QuoteHooks(InvocationContext invocationContext) : base(invocationContext)
    {
    }
    
    [Webhook("On quote deleted", typeof(QuoteDeleteEventHandler), Description = "Triggered when a quote is deleted")]
    public async Task<WebhookResponse<TriggerContent>> QuoteDeleted(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName, Creds);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On quote created", typeof(QuoteCreatedEventHandler), Description = "Triggered when a quote is created")]
    public async Task<WebhookResponse<TriggerContent>> QuoteCreated(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName, Creds);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

    [Webhook("On quote status changed", typeof(QuoteChangedEventHandler), Description = "Triggered when a quote status is changed")]
    public async Task<WebhookResponse<TriggerContent>> QuoteStatusChanged(WebhookRequest webhookRequest)
    {
        var callbackService = new CallbackServiceEmulator(ServiceName, XmlTagName, Creds);
        return await callbackService.HandleWsdlRequset(webhookRequest);
    }

}