using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Xml.Linq;
using System.Xml.Serialization;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Handlers;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using RestSharp;

namespace Blackbird.Plugins.Plunet.Webhooks;

[WebhookList]
public class WebhookList
{
    private const string WsdlService = "https://test71.plunet.com/CallbackOrder30";
    
    [Webhook(typeof(DeleteOrderEventHandler))]
    public async Task<WebhookResponse<ReceiveNotifyCallback>> OrderDeleted(WebhookRequest webhookRequest)
    {
        if (webhookRequest.HttpMethod == HttpMethod.Get)
        {
            return await ProceedPreflightWebhook(webhookRequest);
        }
        var doc = XDocument.Parse(webhookRequest.Body.ToString() ?? string.Empty);
        var element = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName==nameof(ReceiveNotifyCallback));
        if (element == null)
        {
            return new WebhookResponse<ReceiveNotifyCallback>();
        }

        using var xmlReader = element.CreateReader();
        var serializer = new XmlSerializer(typeof(ReceiveNotifyCallback));
        var deleteOrderResponse = (ReceiveNotifyCallback)serializer.Deserialize(xmlReader)!;
        return new WebhookResponse<ReceiveNotifyCallback>
        {
            HttpResponseMessage = null,
            Result = deleteOrderResponse
        };
    }

    private async Task<WebhookResponse<ReceiveNotifyCallback>> ProceedPreflightWebhook(WebhookRequest webhookRequest)
    {
        var url = webhookRequest.Headers.GetValueOrDefault("webhookUrl");
        var client = new RestClient();
        var request = new RestRequest($"{WsdlService}?wsdl");
        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            return new WebhookResponse<ReceiveNotifyCallback>();
        }
        
        var content =
            response.Content?.Replace(WsdlService,
                url)??string.Empty;
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = new StringContent(content);
        response.Headers?.ToList().ForEach(headerParameter =>
            httpResponseMessage.Headers.Add(headerParameter.Name, headerParameter.Value.ToString()));
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Xml);
        httpResponseMessage.StatusCode = response.StatusCode;
        return new WebhookResponse<ReceiveNotifyCallback>
        {
            HttpResponseMessage = httpResponseMessage,
            Result = null
        };
    }
}