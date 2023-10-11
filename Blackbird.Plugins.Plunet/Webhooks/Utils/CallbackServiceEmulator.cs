using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using RestSharp;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Xml.Linq;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;

namespace Blackbird.Plugins.Plunet.Webhooks.Utils;

public class CallbackServiceEmulator
{
    private readonly string _serviceName;
    private readonly string _tagName;
    private string Url { get; }

    private string WsdlServiceUrl => $"{Url}/{_serviceName}";

    public CallbackServiceEmulator(string serviceName, string tagName,
        IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        _serviceName = serviceName;
        _tagName = tagName;
        Url = creds.Get(AppConstants.UrlNameKey).Value;
    }

    public Task<WebhookResponse<TriggerContent>> HandleWsdlRequset(WebhookRequest webhookRequest)
    {
        return webhookRequest.HttpMethod == HttpMethod.Get
            ? GeneratePreflightResponse(webhookRequest)
            : GenerateTriggerResponse(webhookRequest);
    }

    private Task<WebhookResponse<TriggerContent>> GenerateTriggerResponse(WebhookRequest webhookRequest)
    {
        var doc = XDocument.Parse(webhookRequest.Body.ToString() ?? string.Empty);

        var value = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName == _tagName)?.Value;
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = new StringContent(
            "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:api=\"http://API.Integration/\"><soap:Header/><soap:Body><api:receiveNotifyCallbackResponse/></soap:Body></soap:Envelope>");
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Soap);

        return Task.FromResult(new WebhookResponse<TriggerContent>
        {
            HttpResponseMessage = httpResponseMessage,
            Result = value == null ? null : new TriggerContent { Id = value },
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
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = new StringContent(content);
        response.Headers?.ToList().ForEach(headerParameter =>
            httpResponseMessage.Headers.Add(headerParameter.Name ?? string.Empty,
                headerParameter.Value?.ToString()));
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Xml);
        httpResponseMessage.StatusCode = response.StatusCode;

        return new WebhookResponse<TriggerContent>
        {
            HttpResponseMessage = httpResponseMessage,
            Result = null,
            ReceivedWebhookRequestType = WebhookRequestType.Preflight
        };
    }
}