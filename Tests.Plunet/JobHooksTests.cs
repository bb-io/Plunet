using Apps.Plunet.Models.Job;
using Apps.Plunet.Webhooks.WebhookLists;

namespace Tests.Plunet;

[TestClass]
public class JobHooksTests
{
    [TestMethod]
    public void JobHooks_status_changed_matches_exact_full_job_type()
    {
        var result = TriggerJobStatusChanged(new JobResponse
        {
            JobId = "2130122",
            Status = "1",
            JobType = "z_Annuity",
            JobTypeShort = "ANN"
        }, "z_Annuity");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void JobHooks_status_changed_matches_short_job_type()
    {
        var result = TriggerJobStatusChanged(new JobResponse
        {
            JobId = "2130122",
            Status = "1",
            JobType = "z_Annuity",
            JobTypeShort = "ANN"
        }, "ANN");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void JobHooks_status_changed_matches_composite_full_job_type()
    {
        var result = TriggerJobStatusChanged(new JobResponse
        {
            JobId = "2130122",
            Status = "1",
            JobType = "z_Annuity | English (USA)/Arabic",
            JobTypeShort = "ANN"
        }, "z_Annuity");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void JobHooks_status_changed_filters_non_matching_job_type()
    {
        var result = TriggerJobStatusChanged(new JobResponse
        {
            JobId = "2130122",
            Status = "1",
            JobType = "Translation",
            JobTypeShort = "TRA"
        }, "z_Annuity");

        Assert.IsFalse(result);
    }

    private static bool TriggerJobStatusChanged(JobResponse job, string jobType)
        => JobHooks.ShouldTriggerJobStatusChanged(
            job,
            new NewStatusesOptionalRequest { Statuses = ["1"] },
            new GetJobOptionalRequest(),
            new JobTypeOptionRequest { JobType = jobType });
}
