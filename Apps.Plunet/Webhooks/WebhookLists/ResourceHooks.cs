using Apps.Plunet.Actions;
using Apps.Plunet.Constants;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Apps.Plunet.Models.Customer;
using Apps.Plunet.Models.Resource.Response;
using Apps.Plunet.Webhooks.Handlers.Impl.Resources;
using Apps.Plunet.Webhooks.Models;
using Apps.Plunet.Webhooks.WebhookLists.Base;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using System.Xml.Linq;
using Apps.Plunet.Models.Resource.Request;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class ResourceHooks(InvocationContext invocationContext) : PlunetWebhookList<ResourceResponse>(invocationContext)
{
    protected override string ServiceName => "CallbackResource30";
    protected override string TriggerResponse => SoapResponses.CustomerAndResourceOk;

    private const string XmlIdTagName = "ResourceID";

    private ResourceActions Actions { get; set; } = new(invocationContext);

    protected override async Task<ResourceResponse> GetEntity(XDocument doc)
    {
        var id = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName.Equals(XmlIdTagName, StringComparison.OrdinalIgnoreCase))?.Value;
        return await Actions.GetResource(id);
    }

    [Webhook("On resource deleted", typeof(ResourceDeleteEventHandler),
        Description = "Triggered when a resource is deleted")]
    public Task<WebhookResponse<ResourceResponse>> ResourceDeleted(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, resource => true);

    [Webhook("On resource created", typeof(ResourceCreatedEventHandler),
        Description = "Triggered when a resource is created")]
    public Task<WebhookResponse<ResourceResponse>> ResourceCreated(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, resource => true);

    [Webhook("On resource status changed", typeof(ResourceChangedEventHandler),
        Description = "Triggered when a resource status is changed")]
    public Task<WebhookResponse<ResourceResponse>> ResourceChanged(WebhookRequest webhookRequest, 
        [WebhookParameter][Display("New status")][StaticDataSource(typeof(ResourceStatusDataHandler))] string? newStatus,
        [WebhookParameter] GetResourceOptionalRequest optonalResource)
        => HandleWebhook(webhookRequest, resource => (newStatus == null || newStatus == resource.Status) && 
                                                     (optonalResource.ResourceId == null || optonalResource.ResourceId == resource.ResourceID));
}