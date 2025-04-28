using Apps.Plunet.Actions;
using Apps.Plunet.Constants;
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
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class QuoteHooks(InvocationContext invocationContext) : PlunetWebhookList<QuoteResponse>(invocationContext)
{
    protected override string ServiceName => "CallbackQuote30";
    protected override string TriggerResponse => SoapResponses.OtherOk;

    private const string XmlIdTagName = "QuoteID";
    private QuoteActions Actions { get; set; } = new(invocationContext);

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
    public Task<WebhookResponse<QuoteResponse>> QuoteStatusChanged(WebhookRequest webhookRequest,
        [WebhookParameter] [Display("Quote status")] [StaticDataSource(typeof(QuoteStatusDataHandler))] string? newStatus,
        [WebhookParameter] [Display("Project category"),StaticDataSource(typeof(ProjectCategoryDataHandler))] string? category,
        [WebhookParameter] [Display("Project status"), StaticDataSource(typeof(ProjectStatusDataHandler))] string? projectStatus,
        [WebhookParameter] GetQuoteOptionalRequest quoteOptionalRequest)
        => HandleWebhook(webhookRequest,
            quote => (newStatus == null || newStatus == quote.Status) &&
                     (category == null || category == quote.ProjectCategory) && 
                     (projectStatus == null || projectStatus == quote.ProjectStatus) &&
                     (quoteOptionalRequest.QuoteId == null || quoteOptionalRequest.QuoteId == quote.QuoteId));
}