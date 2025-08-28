using Apps.Plunet.Actions;
using Apps.Plunet.Constants;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Apps.Plunet.Models.Order;
using Apps.Plunet.Models.Request.Response;
using Apps.Plunet.Webhooks.Handlers.Impl.Orders;
using Apps.Plunet.Webhooks.Models;
using Apps.Plunet.Webhooks.WebhookLists.Base;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using System.Xml.Linq;
using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class OrderHooks : PlunetWebhookList<OrderResponse>
{
    protected override string ServiceName => "CallbackOrder30";
    protected override string TriggerResponse => SoapResponses.OtherOk;

    private const string XmlIdTagName = "OrderID";
    private OrderActions Actions { get; set; }

    public OrderHooks(InvocationContext invocationContext) : base(invocationContext)
    {
        Actions = new OrderActions(invocationContext);
    }

    protected override async Task<OrderResponse> GetEntity(XDocument doc)
    {
        var id = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName.Equals(XmlIdTagName, StringComparison.OrdinalIgnoreCase))?.Value;
        return await Actions.GetOrder(new OrderRequest { OrderId = id });
    }

    [Webhook("On order deleted", typeof(OrderDeleteEventHandler), Description = "Triggered when an order is deleted")]
    public Task<WebhookResponse<OrderResponse>> OrderDeleted(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, order => true);

    [Webhook("On order created", typeof(OrderCreatedEventHandler), Description = "Triggered when an order is created")]
    public Task<WebhookResponse<OrderResponse>> OrderCreated(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, order => true);

    [Webhook("On order status changed", typeof(OrderChangedEventHandler),
        Description = "Triggered when an order status is changed")]
    public Task<WebhookResponse<OrderResponse>> OrderStatusChanged(WebhookRequest webhookRequest,
        [WebhookParameter] [Display("New status")] [StaticDataSource(typeof(OrderStatusDataHandler))] string? newStatus,
        [WebhookParameter] [Display("Project category")] string? category,
        [WebhookParameter] [Display("Project status"), StaticDataSource(typeof(ProjectStatusDataHandler))] string? projectStatus,
        [WebhookParameter] GetOrderOptionalRequest orderOptionalRequest)
        => HandleWebhook(webhookRequest,
            order => (newStatus == null || newStatus == order.Status) &&
                     (category == null || category == order.ProjectCategory) &&
                     (projectStatus == null || projectStatus == order.ProjectStatus) &&
                     (orderOptionalRequest.OrderId == null || orderOptionalRequest.OrderId == order.OrderId));
}