using System.Net.Http.Headers;
using System.Net.Mime;
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
using RestSharp;

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
        var id = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName.Equals(XmlIdTagName, StringComparison.OrdinalIgnoreCase))?.Value;
        return await Actions.GetQuote(new GetQuoteRequest { QuoteId = id });
    }

    [Webhook("On quote deleted", typeof(QuoteDeleteEventHandler), Description = "Triggered when a quote is deleted")]
    public Task<WebhookResponse<QuoteIdResponse>> QuoteDeleted(WebhookRequest webhookRequest)
        => HandleQuoteWebhook(webhookRequest, quote => true);

    [Webhook("On quote created", typeof(QuoteCreatedEventHandler), Description = "Triggered when a quote is created")]
    public Task<WebhookResponse<QuoteIdResponse>> QuoteCreated(WebhookRequest webhookRequest)
        => HandleQuoteWebhook(webhookRequest, quote => true);

    [Webhook("On quote status changed", typeof(QuoteChangedEventHandler),
        Description = "Triggered when a quote status is changed")]
    public Task<WebhookResponse<QuoteIdResponse>> QuoteStatusChanged(WebhookRequest webhookRequest,
        [WebhookParameter] [Display("Quote status")] [StaticDataSource(typeof(QuoteStatusDataHandler))] string? newStatus,
        [WebhookParameter] [Display("Project category")] string? category,
        [WebhookParameter] [Display("Project status"), StaticDataSource(typeof(ProjectStatusDataHandler))] string? projectStatus,
        [WebhookParameter] GetQuoteOptionalRequest quoteOptionalRequest)
        => HandleQuoteWebhook(webhookRequest,
            quote => /*(newStatus == null || newStatus == quote.Status) &&
                     (category == null || category == quote.ProjectCategory) && 
                     (projectStatus == null || projectStatus == quote.ProjectStatus) &&*/
                     (quoteOptionalRequest.QuoteId == null || quoteOptionalRequest.QuoteId == quote.QuoteId));
    
    private async Task<WebhookResponse<QuoteIdResponse>> HandleQuoteWebhook(WebhookRequest webhookRequest, Func<QuoteIdResponse, bool> preflightComparisonCheck)
    {
        try 
        {
            return webhookRequest.HttpMethod == HttpMethod.Get 
                ? await GenerateQuotePreflightResponse(webhookRequest)
                : await GenerateQuoteTriggerResponse(webhookRequest);
        }
        catch (Exception ex)
        {
            var errorMessage = "[Plunet webhook] Got an error while processing the webhook request. " 
                               + $"Request method: {webhookRequest.HttpMethod?.Method}"
                               + $"Request body: {webhookRequest.Body}"
                               + $"Service: {ServiceName}"
                               + $"Wsdl service url: {WsdlServiceUrl}"
                               + $"Exception message: {ex.Message}";

            InvocationContext.Logger?.LogError(errorMessage, [ex.Message]);
            throw;
        }
    }
    
    private Task<WebhookResponse<QuoteIdResponse>> GenerateQuoteTriggerResponse(WebhookRequest webhookRequest)
    {
        var doc = XDocument.Parse(webhookRequest.Body.ToString() ?? string.Empty);
        var httpResponseMessage = new HttpResponseMessage()
        {
            Content = new StringContent(TriggerResponse)
        };

        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Soap);
        
        var id = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName.Equals(XmlIdTagName, StringComparison.OrdinalIgnoreCase))?.Value;
        return Task.FromResult<WebhookResponse<QuoteIdResponse>>(new()
        {
            HttpResponseMessage = httpResponseMessage,
            Result = new QuoteIdResponse()
            {
                QuoteId = id ?? string.Empty
            },
            ReceivedWebhookRequestType = WebhookRequestType.Default
        });
    }
    
    private async Task<WebhookResponse<QuoteIdResponse>> GenerateQuotePreflightResponse(WebhookRequest webhookRequest)
    {
        var webhookUrl = webhookRequest.Headers.GetValueOrDefault("webhookUrl");

        using var client = new RestClient();
        var request = new RestRequest($"{WsdlServiceUrl}?wsdl");

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            InvocationContext.Logger?.LogError($"[Plunet webhook] Got an error while fetching the WSDL service ({WsdlServiceUrl}). Service: {ServiceName}; Response status code: {response.StatusCode}", [WsdlServiceUrl]);

            return new()
            {
                HttpResponseMessage = new HttpResponseMessage()
                {
                    Content = new StringContent(response.Content ?? string.Empty),
                    StatusCode = System.Net.HttpStatusCode.OK
                },
                Result = null,
                ReceivedWebhookRequestType = WebhookRequestType.Preflight
            };
        }

        var content = response.Content?.Replace(WsdlServiceUrl, webhookUrl) ?? string.Empty;
        var httpResponseMessage = new HttpResponseMessage()
        {
            Content = new StringContent(content),
            StatusCode = response.StatusCode
        };

        response.Headers?.Where(x => x.Name != null && !x.Name.Contains("Transfer")).ToList().ForEach(headerParameter =>
        {
            httpResponseMessage.Headers.Add(headerParameter.Name ?? string.Empty, headerParameter.Value?.ToString());
        });

        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Xml);

        return new()
        {
            HttpResponseMessage = httpResponseMessage,
            Result = null,
            ReceivedWebhookRequestType = WebhookRequestType.Preflight
        };
    }
}