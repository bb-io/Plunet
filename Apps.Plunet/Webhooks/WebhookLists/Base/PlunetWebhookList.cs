using System.Net.Http.Headers;
using System.Net.Mime;
using System.Xml.Linq;
using Apps.Plunet.Invocables;
using Apps.Plunet.Utils;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using RestSharp;

namespace Apps.Plunet.Webhooks.WebhookLists.Base;

public abstract class PlunetWebhookList<T>(InvocationContext invocationContext) : PlunetInvocable(invocationContext) where T : class
{
    protected abstract string ServiceName { get; }

    protected abstract string TriggerResponse { get; }

    protected abstract Task<T> GetEntity(XDocument doc);

    private string WsdlServiceUrl => $"{Creds.GetUrl()}/{ServiceName}";

    protected Task<WebhookResponse<T>> HandleWebhook(WebhookRequest webhookRequest, Func<T, bool> preflightComparisonCheck)
        => webhookRequest.HttpMethod == HttpMethod.Get
            ? GeneratePreflightResponse(webhookRequest)
            : GenerateTriggerResponse(webhookRequest, preflightComparisonCheck);

    private async Task<WebhookResponse<T>> GenerateTriggerResponse(WebhookRequest webhookRequest, Func<T, bool> preflightComparisonCheck)
    {
        var doc = XDocument.Parse(webhookRequest.Body.ToString() ?? string.Empty);
        var httpResponseMessage = new HttpResponseMessage()
        {
            Content = new StringContent(TriggerResponse)
        };

        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Soap);

        var entity = await GetEntity(doc);
        return new()
        {
            HttpResponseMessage = httpResponseMessage,
            Result = entity,
            ReceivedWebhookRequestType = preflightComparisonCheck(entity) ? WebhookRequestType.Default : WebhookRequestType.Preflight
        };
    }

    private async Task<WebhookResponse<T>> GeneratePreflightResponse(WebhookRequest webhookRequest)
    {
        var webhookUrl = webhookRequest.Headers.GetValueOrDefault("webhookUrl");

        using var client = new RestClient();
        var request = new RestRequest($"{WsdlServiceUrl}?wsdl");

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessStatusCode)
        {
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