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
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class ItemHooks : PlunetWebhookList<ItemResponse>
{
    protected override string ServiceName => "CallbackItem30";
    protected override string TriggerResponse => SoapResponses.OtherOk;


    private const string XmlIdTagName = "ItemID";

    private ItemActions Actions { get; set; }

    public ItemHooks(InvocationContext invocationContext) : base(invocationContext)
    {
        Actions = new ItemActions(invocationContext);
    }

    // NOTE: Currently only works for order items.
    protected override async Task<ItemResponse> GetEntity(XDocument doc)
    {
        var id = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName == XmlIdTagName)?.Value;
        return await Actions.GetItem(new ProjectTypeRequest { ProjectType = "3" }, new GetItemRequest { ItemId = id }, new OptionalCurrencyTypeRequest { });
    }

    [Webhook("On item deleted", typeof(ItemDeleteEventHandler), Description = "Triggered when an item is deleted")]
    public Task<WebhookResponse<ItemResponse>> ItemDeleted(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, item => true);

    [Webhook("On item created", typeof(ItemCreatedEventHandler), Description = "Triggered when an item is created")]
    public Task<WebhookResponse<ItemResponse>> ItemCreated(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, item => true);

    [Webhook("On item status changed", typeof(ItemChangedEventHandler),
        Description = "Triggered when an item status is changed")]
    public Task<WebhookResponse<ItemResponse>> ItemStatusChanged(WebhookRequest webhookRequest, [WebhookParameter][Display("New status")][StaticDataSource(typeof(ItemStatusDataHandler))] string? newStatus)
        => HandleWebhook(webhookRequest, item => newStatus == null || newStatus == item.Status);

    [Webhook("On item delivery date changed", typeof(ItemDeliveryDateChangedEventHandler),
        Description = "Triggered when an item delivery date is changed")]
    public Task<WebhookResponse<ItemResponse>> ItemDeliveryDateChanged(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, item => true);
}