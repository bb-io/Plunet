using Apps.Plunet.Actions;
using Apps.Plunet.Constants;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Apps.Plunet.Models.Quote.Request;
using Apps.Plunet.Models.Quote.Response;
using Apps.Plunet.Webhooks.Handlers.Impl.Quotes;
using Apps.Plunet.Webhooks.WebhookLists.Base;
using Blackbird.Applications.Sdk.Common;
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

    protected override string XmlIdTagName => "QuoteID";
    
    private QuoteActions Actions { get; set; } = new(invocationContext);

    protected override async Task<QuoteResponse?> GetEntity(XDocument doc, string id)
    {
        return await Actions.GetQuote(new GetQuoteRequest { QuoteId = id });
    }

    private static Task<QuoteResponse?> GetEntityIdOnly(XDocument doc, string id) 
        => Task.FromResult<QuoteResponse?>(new QuoteResponse(id));

    [Webhook("On quote deleted", typeof(QuoteDeleteEventHandler), Description = "Triggered when a quote is deleted")]
    public Task<WebhookResponse<QuoteResponse>> QuoteDeleted(WebhookRequest webhookRequest,
        [WebhookParameter] QuoteWebhookRequest request)
        => HandleWebhook(webhookRequest, _ => true, PickGetter(request));

    [Webhook("On quote created", typeof(QuoteCreatedEventHandler), Description = "Triggered when a quote is created")]
    public Task<WebhookResponse<QuoteResponse>> QuoteCreated(WebhookRequest webhookRequest,
        [WebhookParameter] QuoteWebhookRequest request)
        => HandleWebhook(webhookRequest, _ => true, PickGetter(request));

    [Webhook("On quote status changed", typeof(QuoteChangedEventHandler),
        Description = "Triggered when a quote status is changed")]
    public Task<WebhookResponse<QuoteResponse>> QuoteStatusChanged(WebhookRequest webhookRequest,
        [WebhookParameter] [Display("Quote status")] [StaticDataSource(typeof(QuoteStatusDataHandler))] string? newStatus,
        [WebhookParameter] [Display("Project category")] string? category,
        [WebhookParameter] [Display("Project status"), StaticDataSource(typeof(ProjectStatusDataHandler))] string? projectStatus,
        [WebhookParameter] GetQuoteOptionalRequest quoteOptionalRequest,
        [WebhookParameter] QuoteWebhookRequest request)
        => HandleWebhook(webhookRequest,
            quote => (request.ReturnIdOnly == true || (newStatus == null || newStatus == quote.Status)) &&
                     (request.ReturnIdOnly == true || (category == null || category == quote.ProjectCategory)) &&
                     (request.ReturnIdOnly == true || (projectStatus == null || projectStatus == quote.ProjectStatus)) &&
                     (quoteOptionalRequest.QuoteId == null || quoteOptionalRequest.QuoteId == quote.QuoteId),
            PickGetter(request));

    private Func<XDocument, string, Task<QuoteResponse?>> PickGetter(QuoteWebhookRequest request)
        => request.ReturnIdOnly == true ? GetEntityIdOnly : GetEntity;
}
