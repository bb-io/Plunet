using System.Net.Http.Headers;
using System.Net.Mime;
using System.Xml.Linq;
using Apps.Plunet.Extensions;
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

    protected abstract Task<T?> GetEntity(XDocument doc, string id);

    private string WsdlServiceUrl => $"{Creds.GetUrl()}/{ServiceName}";
    
    protected abstract string XmlIdTagName { get; }

    protected Task<WebhookResponse<T>> HandleWebhook(WebhookRequest webhookRequest, Func<T, bool> preflightComparisonCheck)
        => HandleWebhook(webhookRequest, preflightComparisonCheck, GetEntity);

    protected async Task<WebhookResponse<T>> HandleWebhook(
        WebhookRequest webhookRequest, 
        Func<T, bool> preflightComparisonCheck, 
        Func<XDocument, string, Task<T?>> entityGetter)
    {
        try
        {
            return webhookRequest.HttpMethod == HttpMethod.Get
                ? await GeneratePreflightResponse(webhookRequest)
                : await GenerateTriggerResponse(webhookRequest, preflightComparisonCheck, entityGetter);
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

    private async Task<WebhookResponse<T>> GenerateTriggerResponse(
        WebhookRequest webhookRequest, 
        Func<T, bool> preflightComparisonCheck, 
        Func<XDocument, string, Task<T?>> entityGetter)
    {
        var httpResponseMessage = new HttpResponseMessage { Content = new StringContent(TriggerResponse) };
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Soap);
        
        var body = webhookRequest.Body?.ToString();
        if (string.IsNullOrWhiteSpace(body))
        {
            return new()
            {
                HttpResponseMessage = httpResponseMessage,
                Result = null,
                ReceivedWebhookRequestType = WebhookRequestType.Preflight
            };
        }
        
        var doc = XDocument.Parse(body);
        string? id = doc.GetElementValue(XmlIdTagName);
        if (string.IsNullOrWhiteSpace(id))
        {
            InvocationContext.Logger?.LogError(
                $"[Plunet webhook] No {XmlIdTagName} in the callback body for {ServiceName}. Body: {doc.ToString(SaveOptions.DisableFormatting)}", 
                []);

            return new()
            {
                HttpResponseMessage = httpResponseMessage,
                Result = null,
                ReceivedWebhookRequestType = WebhookRequestType.Preflight
            };
        }

        T? entity;
        try
        {
            entity = await entityGetter(doc, id);
        }
        catch (Exception ex) when (ex.IsCantFindError())
        {
            InvocationContext.Logger?.LogError(
                $"[Plunet webhook] Entity {id} no longer exists when handling the {ServiceName} callback, skipping. " +
                $"Body: {doc.ToString(SaveOptions.DisableFormatting)}; Exception: {ex.Message}", []);
            entity = null;
        }

        if (entity is null)
        {
            return new()
            {
                HttpResponseMessage = httpResponseMessage,
                Result = null,
                ReceivedWebhookRequestType = WebhookRequestType.Preflight
            };
        }
        
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