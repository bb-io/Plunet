using System.Net.Http.Headers;
using System.Net.Mime;
using System.Xml.Linq;
using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace Apps.Plunet.Webhooks.WebhookLists.Base;

public abstract class PlunetWebhookList<T> : PlunetInvocable where T : class
{
    protected abstract string ServiceName { get; }

    protected abstract Task<T> GetEntity(XDocument doc);

    private string WsdlServiceUrl => $"{Creds.Get(CredsNames.UrlNameKey).Value}/{ServiceName}";

    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;

    protected PlunetWebhookList(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    protected Task<WebhookResponse<T>> HandleWebhook(WebhookRequest webhookRequest, Func<T, bool> preflightComparisonCheck)
        => webhookRequest.HttpMethod == HttpMethod.Get
            ? GeneratePreflightResponse(webhookRequest)
            : GenerateTriggerResponse(webhookRequest, preflightComparisonCheck);

    private async Task<WebhookResponse<T>> GenerateTriggerResponse(WebhookRequest webhookRequest, Func<T, bool> preflightComparisonCheck)
    {
        var doc = XDocument.Parse(webhookRequest.Body.ToString() ?? string.Empty);
        var triggerResponse =
            "<S:Envelope xmlns:S=\"http://www.w3.org/2003/05/soap-envelope\">\r\n   <S:Body>\r\n      <ns2:ReceiveNotifyCallbackResponse xmlns:ns2=\"http://API.Integration/\"/>\r\n   </S:Body>\r\n</S:Envelope>";

        var httpResponseMessage = new HttpResponseMessage()
        {
            Content = new StringContent(triggerResponse)
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
            return new();

        var content = response.Content?.Replace(WsdlServiceUrl, webhookUrl) ?? string.Empty;
        var httpResponseMessage = new HttpResponseMessage()
        {
            Content = new StringContent(content),
            StatusCode = response.StatusCode
        };
        response.Headers?.ToList().ForEach(headerParameter =>
            httpResponseMessage.Headers.Add(headerParameter.Name ?? string.Empty, headerParameter.Value?.ToString()));

        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Xml);

        return new()
        {
            HttpResponseMessage = httpResponseMessage,
            Result = null,
            ReceivedWebhookRequestType = WebhookRequestType.Preflight
        };
    }
}