using Apps.Plunet.Actions;
using Apps.Plunet.Constants;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Apps.Plunet.Models.Job;
using Apps.Plunet.Models.Quote.Request;
using Apps.Plunet.Models.Quote.Response;
using Apps.Plunet.Webhooks.Handlers.Impl.Jobs;
using Apps.Plunet.Webhooks.Models;
using Apps.Plunet.Webhooks.WebhookLists.Base;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using System.Xml.Linq;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Webhooks.WebhookLists;

[WebhookList]
public class JobHooks : PlunetWebhookList<JobResponse>
{
    protected override string ServiceName => "CallbackJob30";
    protected override string TriggerResponse => SoapResponses.OtherOk;

    private const string XmlIdTagName = "JobID";
    private const string XmlProjectTagName = "ProjectType";

    private JobActions Actions { get; set; }

    public JobHooks(InvocationContext invocationContext) : base(invocationContext)
    {
        Actions = new JobActions(invocationContext);
    }

    protected override async Task<JobResponse> GetEntity(XDocument doc)
    {
        var id = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName == XmlIdTagName)?.Value;
        var projectType = doc.Elements().Descendants().FirstOrDefault(x => x.Name.LocalName == XmlProjectTagName)?.Value;
        return await Actions.GetJob(new GetJobRequest { JobId = id, ProjectType = projectType });
    }

    [Webhook("On job deleted", typeof(JobDeleteEventHandler), Description = "Triggered when a job is deleted")]
    public Task<WebhookResponse<JobResponse>> JobDeleted(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, job => true);

    [Webhook("On job created", typeof(JobCreatedEventHandler), Description = "Triggered when a job is created")]
    public Task<WebhookResponse<JobResponse>> JobCreated(WebhookRequest webhookRequest)
        => HandleWebhook(webhookRequest, job => true);

    [Webhook("On job status changed", typeof(JobChangedEventHandler),
        Description = "Triggered when a job status is changed")]
    public Task<WebhookResponse<JobResponse>> JobStatusChanged(WebhookRequest webhookRequest, 
        [WebhookParameter] NewStatusesOptionalRequest newStatusRequest,
        [WebhookParameter] GetJobOptionalRequest request) => HandleWebhook(webhookRequest, job => (newStatusRequest.Statuses == null || !newStatusRequest.Statuses.Any() || newStatusRequest.Statuses.Contains(job.Status))
                                            && (request.JobId == null || request.JobId == job.JobId));


    [Webhook("On job delivery date changed", typeof(JobDeliveryDateChangedEventHandler),
        Description = "Triggered when a job delivery date is changed")]
    public Task<WebhookResponse<JobResponse>> JobDeliveryDateChanged(WebhookRequest webhookRequest,
        [WebhookParameter] GetJobOptionalRequest request)
        => HandleWebhook(webhookRequest, job => request.JobId == null || request.JobId == job.JobId);

    [Webhook("On job start date changed", typeof(JobStartDateChangedEventHandler),
        Description = "Triggered when a job start date is changed")]
    public Task<WebhookResponse<JobResponse>> JobStartDateChanged(WebhookRequest webhookRequest,
        [WebhookParameter] GetJobOptionalRequest request)
        => HandleWebhook(webhookRequest, job => request.JobId == null || request.JobId == job.JobId);
}