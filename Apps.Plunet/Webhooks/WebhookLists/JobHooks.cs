using Apps.Plunet.Actions;
using Apps.Plunet.Constants;
using Apps.Plunet.Models.Job;
using Apps.Plunet.Webhooks.Handlers.Impl.Jobs;
using Apps.Plunet.Webhooks.WebhookLists.Base;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using System.Xml.Linq;
using Apps.Plunet.Extensions;

namespace Apps.Plunet.Webhooks.WebhookLists;

[WebhookList("Jobs")]
public class JobHooks(InvocationContext invocationContext) : PlunetWebhookList<JobResponse>(invocationContext)
{
    protected override string ServiceName => "CallbackJob30";
    protected override string TriggerResponse => SoapResponses.OtherOk;

    protected override string XmlIdTagName => "JobID";
    
    private const string XmlProjectTagName = "ProjectType";

    private JobActions Actions { get; set; } = new(invocationContext);

    protected override async Task<JobResponse?> GetEntity(XDocument doc, string id)
    {
        string? projectType = doc.GetElementValue(XmlProjectTagName);
        if (string.IsNullOrWhiteSpace(projectType))
        {
            InvocationContext.Logger?.LogError($"[JobHooks] Could not find {XmlProjectTagName} in the webhook request. Request: {doc}", []);
            return null;
        }

        try
        {
            return await Actions.GetJob(new GetJobRequest { JobId = id, ProjectType = projectType });
        }
        catch (Exception ex) when (ex.Message.Contains("can't find the requested job", StringComparison.OrdinalIgnoreCase))
        {
            InvocationContext.Logger?.LogError($"[JobHooks] Job {id} (ProjectType {projectType}) no longer exists when handling callback - skipping. Request: {doc}", []);
            return null!;
        }
        catch (Exception)
        {
            InvocationContext.Logger?.LogError($"[JobHooks] Error getting job with ID {id} and ProjectType {projectType}. Request: {doc}", []);
            throw;
        }
    }

    private static Task<JobResponse?> GetEntityIdOnly(XDocument doc, string id)
        => Task.FromResult<JobResponse?>(new JobResponse(id));

    private Func<XDocument, string, Task<JobResponse?>> PickGetter(JobWebhookRequest request)
        => request.ReturnIdOnly == true ? GetEntityIdOnly : GetEntity;

    [Webhook("On job deleted", typeof(JobDeleteEventHandler), Description = "Triggered when a job is deleted")]
    public Task<WebhookResponse<JobResponse>> JobDeleted(WebhookRequest webhookRequest,
        [WebhookParameter] JobWebhookRequest request)
        => HandleWebhook(webhookRequest, _ => true, PickGetter(request));

    [Webhook("On job created", typeof(JobCreatedEventHandler), Description = "Triggered when a job is created")]
    public Task<WebhookResponse<JobResponse>> JobCreated(WebhookRequest webhookRequest,
        [WebhookParameter] JobWebhookRequest request)
        => HandleWebhook(webhookRequest, _ => true, PickGetter(request));

    [Webhook("On job status changed", typeof(JobChangedEventHandler),
        Description = "Triggered when a job status is changed")]
    public Task<WebhookResponse<JobResponse>> JobStatusChanged(WebhookRequest webhookRequest,
        [WebhookParameter] NewStatusesOptionalRequest newStatusRequest,
        [WebhookParameter] GetJobOptionalRequest request,
        [WebhookParameter] JobTypeOptionRequest jobtype,
        [WebhookParameter] JobWebhookRequest jobWebhookRequest)
        => HandleWebhook(webhookRequest,
            job => ShouldTriggerJobStatusChanged(job, newStatusRequest, request, jobtype, jobWebhookRequest),
            PickGetter(jobWebhookRequest));

    [Webhook("On job delivery date changed", typeof(JobDeliveryDateChangedEventHandler),
        Description = "Triggered when a job delivery date is changed")]
    public Task<WebhookResponse<JobResponse>> JobDeliveryDateChanged(WebhookRequest webhookRequest,
        [WebhookParameter] GetJobOptionalRequest request,
        [WebhookParameter] JobWebhookRequest jobWebhookRequest)
        => HandleWebhook(webhookRequest,
            job => request.JobId == null || request.JobId == job.JobId,
            PickGetter(jobWebhookRequest));

    [Webhook("On job start date changed", typeof(JobStartDateChangedEventHandler),
        Description = "Triggered when a job start date is changed")]
    public Task<WebhookResponse<JobResponse>> JobStartDateChanged(WebhookRequest webhookRequest,
        [WebhookParameter] GetJobOptionalRequest request,
        [WebhookParameter] JobWebhookRequest jobWebhookRequest)
        => HandleWebhook(webhookRequest,
            job => request.JobId == null || request.JobId == job.JobId,
            PickGetter(jobWebhookRequest));

    public static bool ShouldTriggerJobStatusChanged(
        JobResponse job,
        NewStatusesOptionalRequest newStatusRequest,
        GetJobOptionalRequest request,
        JobTypeOptionRequest jobtype,
        JobWebhookRequest? jobWebhookRequest = null)
        => (jobWebhookRequest?.ReturnIdOnly == true || newStatusRequest.Statuses == null || !newStatusRequest.Statuses.Any() || newStatusRequest.Statuses.Contains(job.Status))
           && (request.JobId == null || request.JobId == job.JobId)
           && (jobWebhookRequest?.ReturnIdOnly == true || MatchesJobType(jobtype.JobType, job));

    private static bool MatchesJobType(string? configuredJobType, JobResponse job)
    {
        if (string.IsNullOrWhiteSpace(configuredJobType))
        {
            return true;
        }

        var filter = configuredJobType.Trim();
        return IsSameJobType(filter, job.JobType) || IsSameJobType(filter, job.JobTypeShort);
    }

    private static bool IsSameJobType(string filter, string? jobType)
    {
        if (string.IsNullOrWhiteSpace(jobType))
        {
            return false;
        }

        var normalizedJobType = jobType.Trim();
        return string.Equals(filter, normalizedJobType, StringComparison.OrdinalIgnoreCase)
               || normalizedJobType.StartsWith($"{filter} |", StringComparison.OrdinalIgnoreCase);
    }
}
