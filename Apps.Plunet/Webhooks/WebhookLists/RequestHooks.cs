using Apps.Plunet.Actions;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Apps.Plunet.Models.Customer;
using Apps.Plunet.Models.Request.Response;
using Apps.Plunet.Webhooks.Handlers.Impl.Requests;
using Apps.Plunet.Webhooks.Models;
using Apps.Plunet.Webhooks.WebhookLists.Base;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using System.Xml.Linq;

namespace Apps.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class RequestHooks : PlunetWebhookList<RequestResponse>
{
    protected override string ServiceName => "CallbackRequest30";

    private const string XmlIdTagName = "RequestID";

    private RequestActions Actions { get; set; }


    public RequestHooks(InvocationContext invocationContext) : base(invocationContext)
    {
        Actions = new RequestActions(invocationContext);
    }

    protected override async Task<RequestResponse> GetEntity(XDocument doc)
    {
        var id = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName == XmlIdTagName)?.Value;
        return await Actions.GetRequest(id);
    }

    [Webhook("On request deleted", typeof(RequestDeleteEventHandler),
        Description = "Triggered when a request is deleted")]
    public Task<WebhookResponse<RequestResponse>> RequestDeleted(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, request => true);

    [Webhook("On request created", typeof(RequestCreatedEventHandler),
        Description = "Triggered when a request is created")]
    public Task<WebhookResponse<RequestResponse>> RequestCreated(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, request => true);

    [Webhook("On request status changed", typeof(RequestChangedEventHandler),
        Description = "Triggered when a request status is changed")]
    public Task<WebhookResponse<RequestResponse>> RequestChanged(WebhookRequest webhookRequest, [WebhookParameter][Display("New status")][DataSource(typeof(RequestStatusDataHandler))] string? newStatus)
        => HandleWebhook(webhookRequest, request => newStatus == null || newStatus == request.Status);
}