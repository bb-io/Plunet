using System.Net.Http.Headers;
using System.Net.Mime;
using System.Xml.Linq;
using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Webhooks.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace Apps.Plunet.Webhooks.WebhookLists.Base;

public abstract class PlunetWebhookList : PlunetInvocable
{
    protected abstract string ServiceName { get; }
    protected abstract string XmlTagName { get; }

    private string WsdlServiceUrl => $"{Creds.Get(CredsNames.UrlNameKey).Value}/{ServiceName}";

    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;

    protected PlunetWebhookList(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    protected Task<WebhookResponse<TriggerContent>> HandleWebhook(WebhookRequest webhookRequest)
        => webhookRequest.HttpMethod == HttpMethod.Get
            ? GeneratePreflightResponse(webhookRequest)
            : GenerateTriggerResponse(webhookRequest);

    private Task<WebhookResponse<TriggerContent>> GenerateTriggerResponse(WebhookRequest webhookRequest)
    {
        var doc = XDocument.Parse(webhookRequest.Body.ToString() ?? string.Empty);
        var triggerResponse =
            "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:api=\"http://API.Integration/\"><soap:Header/><soap:Body><api:receiveNotifyCallbackResponse/></soap:Body></soap:Envelope>";

        var value = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName == XmlTagName)?.Value;
        var httpResponseMessage = new HttpResponseMessage()
        {
            Content = new StringContent(triggerResponse)
        };
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Soap);

        return Task.FromResult(new WebhookResponse<TriggerContent>
        {
            HttpResponseMessage = httpResponseMessage,
            Result = value == null ? null : new() { Id = value },
            ReceivedWebhookRequestType = WebhookRequestType.Default
        });
    }

    private async Task<WebhookResponse<TriggerContent>> GeneratePreflightResponse(WebhookRequest webhookRequest)
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