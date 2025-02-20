using System.Xml.Linq;
using Apps.Plunet.Actions;
using Apps.Plunet.Constants;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Apps.Plunet.Models.Request.Request;
using Apps.Plunet.Models.Request.Response;
using Apps.Plunet.Webhooks.Handlers.Impl.Requests;
using Apps.Plunet.Webhooks.Models.Parameters;
using Apps.Plunet.Webhooks.WebhookLists.Base;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class RequestHooks(InvocationContext invocationContext) : PlunetWebhookList<RequestResponse>(invocationContext)
{
    protected override string ServiceName => "CallbackRequest30";
    protected override string TriggerResponse => SoapResponses.OtherOk;


    private const string XmlIdTagName = "RequestID";

    private RequestActions Actions { get; set; } = new(invocationContext);


    protected override async Task<RequestResponse> GetEntity(XDocument doc)
    {
        var id = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName == XmlIdTagName)?.Value;
        return await Actions.GetRequest(id);
    }

    [Webhook("On request deleted", typeof(RequestDeleteEventHandler),
        Description = "Triggered when a request is deleted")]
    public Task<WebhookResponse<RequestResponse>> RequestDeleted(WebhookRequest webhookRequest,
        [WebhookParameter] CustomerIdFilter customerIdFilter)
        => HandleWebhook(webhookRequest, request =>
               customerIdFilter == null || customerIdFilter.CustomerId == request.CustomerId);

    [Webhook("On request created", typeof(RequestCreatedEventHandler),
        Description = "Triggered when a request is created")]
    public Task<WebhookResponse<RequestResponse>> RequestCreated(WebhookRequest webhookRequest, 
        [WebhookParameter] CustomerIdFilter customerIdFilter)
        => HandleWebhook(webhookRequest, request =>
               customerIdFilter == null || customerIdFilter.CustomerId == request.CustomerId);

    [Webhook("On request status changed", typeof(RequestChangedEventHandler),
        Description = "Triggered when a request status is changed")]
    public Task<WebhookResponse<RequestResponse>> RequestChanged(WebhookRequest webhookRequest,
        [WebhookParameter][Display("New status")][StaticDataSource(typeof(RequestStatusDataHandler))] string? newStatus,
        [WebhookParameter] GetRequestOptionalRequest optonalRequest, [WebhookParameter] CustomerIdFilter customerIdFilter)
        => HandleWebhook(webhookRequest, request => (newStatus == null || newStatus == request.Status) &&
                                                    (optonalRequest.RequestId == null || optonalRequest.RequestId == request.RequestId) &&
                                                    (customerIdFilter == null || customerIdFilter.CustomerId == request.CustomerId));
}