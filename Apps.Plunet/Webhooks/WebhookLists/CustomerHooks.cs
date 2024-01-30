using Apps.Plunet.Actions;
using Apps.Plunet.Constants;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Apps.Plunet.Models.Customer;
using Apps.Plunet.Webhooks.Handlers.Impl.Customers;
using Apps.Plunet.Webhooks.Models;
using Apps.Plunet.Webhooks.WebhookLists.Base;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using System.Xml.Linq;

namespace Apps.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class CustomerHooks : PlunetWebhookList<GetCustomerResponse>
{
    protected override string ServiceName => "CallbackCustomer30";

    protected override string TriggerResponse => SoapResponses.CustomerAndResourceOk;

    private const string XmlIdTagName = "CustomerID";

    private CustomerActions Actions { get; set; }

    public CustomerHooks(InvocationContext invocationContext) : base(invocationContext)
    {
        Actions = new CustomerActions(invocationContext);
    }

    protected override async Task<GetCustomerResponse> GetEntity(XDocument doc)
    {
        var id = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName == XmlIdTagName)?.Value;
        return await Actions.GetCustomerById(new CustomerRequest { CustomerId = id });
    }

    [Webhook("On customer deleted", typeof(CustomerDeleteEventHandler),
        Description = "Triggered when a customer is deleted")]
    public Task<WebhookResponse<GetCustomerResponse>> CustomerDeleted(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, customer => true);

    [Webhook("On customer created", typeof(CustomerCreatedEventHandler),
        Description = "Triggered when a customer is created")]
    public Task<WebhookResponse<GetCustomerResponse>> CustomerCreated(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, customer => true);

    [Webhook("On customer status changed", typeof(CustomerChangedEventHandler),
        Description = "Triggered when a customer status is changed")]
    public Task<WebhookResponse<GetCustomerResponse>> CustomerChanged(WebhookRequest webhookRequest, [WebhookParameter][Display("New status")][DataSource(typeof(CustomerStatusDataHandler))] string? newStatus)
        => HandleWebhook(webhookRequest, customer => newStatus == null || newStatus == customer.Status);
}