﻿using Apps.Plunet.Actions;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Apps.Plunet.Models.Order;
using Apps.Plunet.Models.Quote.Request;
using Apps.Plunet.Models.Quote.Response;
using Apps.Plunet.Webhooks.Handlers.Impl.Quotes;
using Apps.Plunet.Webhooks.Models;
using Apps.Plunet.Webhooks.WebhookLists.Base;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using System.Xml.Linq;

namespace Apps.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class QuoteHooks : PlunetWebhookList<QuoteResponse>
{
    protected override string ServiceName => "CallbackQuote30";
    private const string XmlIdTagName = "QuoteID";
    private QuoteActions Actions { get; set; }

    public QuoteHooks(InvocationContext invocationContext) : base(invocationContext)
    {
        Actions = new QuoteActions(invocationContext);
    }

    protected override async Task<QuoteResponse> GetEntity(XDocument doc)
    {
        var id = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName == XmlIdTagName)?.Value;
        return await Actions.GetQuote(new GetQuoteRequest { QuoteId = id });
    }

    [Webhook("On quote deleted", typeof(QuoteDeleteEventHandler), Description = "Triggered when a quote is deleted")]
    public Task<WebhookResponse<QuoteResponse>> QuoteDeleted(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, quote => true);

    [Webhook("On quote created", typeof(QuoteCreatedEventHandler), Description = "Triggered when a quote is created")]
    public Task<WebhookResponse<QuoteResponse>> QuoteCreated(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, quote => true);

    [Webhook("On quote status changed", typeof(QuoteChangedEventHandler),
        Description = "Triggered when a quote status is changed")]
    public Task<WebhookResponse<QuoteResponse>> QuoteStatusChanged(WebhookRequest webhookRequest, [WebhookParameter][Display("New status")][DataSource(typeof(QuoteStatusDataHandler))] string? newStatus)
        => HandleWebhook(webhookRequest, quote => newStatus == null || newStatus == quote.Status);
}