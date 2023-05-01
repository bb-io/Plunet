using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.Webhooks.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Blackbird.Plugins.Plunet.Webhooks.Utils
{
    public class CallbackServiceEmulator<T> where T : ITriggerableCallback
    {
        private string _serviceName;
        private const string blackbirdPlunetUrl = "https://test71.plunet.com/";

        private string WsdlServiceUrl => $"{blackbirdPlunetUrl}${_serviceName}";

        public CallbackServiceEmulator(string serviceName)
        {
            _serviceName= serviceName;
        }

        public Task<WebhookResponse<TriggerContent>> HandleWsdlRequset(WebhookRequest webhookRequest)
        {
            if (webhookRequest.HttpMethod == HttpMethod.Get)
            {
                return GeneratePreflightResponse(webhookRequest);
            }
            return GenerateTriggerResponse(webhookRequest);
        }

        private async Task<WebhookResponse<TriggerContent>> GenerateTriggerResponse(WebhookRequest webhookRequest)
        {
            var doc = XDocument.Parse(webhookRequest.Body.ToString() ?? string.Empty);
            var element = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName == nameof(T));

            if (element == null)
            {
                return new WebhookResponse<TriggerContent>();
            }

            using var xmlReader = element.CreateReader();
            var serializer = new XmlSerializer(typeof(T));
            var response = (T)serializer.Deserialize(xmlReader)!;
            var triggerResponse = response.GetTriggerContent();
            return new WebhookResponse<TriggerContent>
            {
                HttpResponseMessage = null,
                Result = triggerResponse
            };
        }
          

        private async Task<WebhookResponse<TriggerContent>> GeneratePreflightResponse(WebhookRequest webhookRequest)
        {
            var webhookUrl = webhookRequest.Headers.GetValueOrDefault("webhookUrl");
            var client = new RestClient();
            var request = new RestRequest($"{WsdlServiceUrl}?wsdl");
            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful)
            {
                return new WebhookResponse<TriggerContent>();
            }

            var content = response.Content?.Replace(WsdlServiceUrl, webhookUrl) ?? string.Empty;
            var httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.Content = new StringContent(content);
            response.Headers?.ToList().ForEach(headerParameter => httpResponseMessage.Headers.Add(headerParameter.Name ?? string.Empty, headerParameter.Value?.ToString()));
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Xml);
            httpResponseMessage.StatusCode = response.StatusCode;

            return new WebhookResponse<TriggerContent>
            {
                HttpResponseMessage = httpResponseMessage,
                Result = null
            };
        }

    }
}
