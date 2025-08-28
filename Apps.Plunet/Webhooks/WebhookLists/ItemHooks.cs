using Apps.Plunet.Actions;
using Apps.Plunet.Constants;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Apps.Plunet.Models.Customer;
using Apps.Plunet.Models.Item;
using Apps.Plunet.Webhooks.Handlers.Impl.Items;
using Apps.Plunet.Webhooks.Models;
using Apps.Plunet.Webhooks.WebhookLists.Base;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using System.Xml.Linq;
using Apps.Plunet.Models;
using Apps.Plunet.Webhooks.Models.Parameters;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class ItemHooks(InvocationContext invocationContext) : PlunetWebhookList<ItemResponse>(invocationContext)
{
    private const string XmlIdTagName = "ItemID";
    private const string XmlProjectTypeTagName = "ProjectType";
    
    protected override string ServiceName => "CallbackItem30";
    protected override string TriggerResponse => SoapResponses.ItemCallbackOk;

    private ItemActions Actions { get; set; } = new(invocationContext);

    protected override async Task<ItemResponse> GetEntity(XDocument doc)
    {
        try
        {
            var id = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName == XmlIdTagName)?.Value!;
            var projectType = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName.Equals(XmlProjectTypeTagName, StringComparison.OrdinalIgnoreCase))?.Value!;
            return await Actions.GetItem(new ProjectTypeRequest { ProjectType = projectType }, new GetItemRequest { ItemId = id }, new OptionalCurrencyTypeRequest { });
        }
        catch (Exception ex)
        {
            var errorMessage = "[Plunet webhook] Got an error while getting the item entity. " 
                + $"Request body: {doc.Document?.ToString(SaveOptions.DisableFormatting)}"
                + $"Exception message: {ex.Message}";

            InvocationContext.Logger?.LogError(errorMessage, [ex.Message]);
            throw;
        }
    }

    [Webhook("On item deleted", typeof(ItemDeleteEventHandler), Description = "Triggered when an item is deleted")]
    public Task<WebhookResponse<ItemResponse>> ItemDeleted(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, item => true);

    [Webhook("On item created", typeof(ItemCreatedEventHandler), Description = "Triggered when an item is created")]
    public Task<WebhookResponse<ItemResponse>> ItemCreated(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, item => true);

    [Webhook("On item status changed", typeof(ItemChangedEventHandler),
        Description = "Triggered when an item status is changed")]
    public Task<WebhookResponse<ItemResponse>> ItemStatusChanged(WebhookRequest webhookRequest, 
        [WebhookParameter][Display("New status")][StaticDataSource(typeof(ItemStatusDataHandler))] string? newStatus,
        [WebhookParameter] GetProjectOptionalRequest projectRequest,
        [WebhookParameter] GetItemOptionalRequest itemRequest, 
        [WebhookParameter] AdditionalFiltersRequests additionalFiltersRequests)
        => HandleWebhook(webhookRequest, item => (newStatus == null || newStatus == item.Status) 
                                                 && (additionalFiltersRequests.DescriptionContains == null || item.BriefDescription
                                                     .Contains(additionalFiltersRequests.DescriptionContains, StringComparison.OrdinalIgnoreCase))
                                                 && (projectRequest.ProjectId == null || projectRequest.ProjectId == item.ProjectId) 
                                                 && (itemRequest.ItemId == null || itemRequest.ItemId == item.ItemId));

    [Webhook("On item delivery date changed", typeof(ItemDeliveryDateChangedEventHandler),
        Description = "Triggered when an item delivery date is changed")]
    public Task<WebhookResponse<ItemResponse>> ItemDeliveryDateChanged(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, item => true);
}