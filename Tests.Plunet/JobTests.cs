using Apps.Plunet.Actions;
using Apps.Plunet.Models.Item;
using Apps.Plunet.Models.Job;
using Apps.Plunet.Webhooks.WebhookLists;
using Blackbird.Applications.Sdk.Common.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tests.Plunet.Base;

namespace Tests.Plunet;
[TestClass]
public class JobTests : TestBase
{
    [TestMethod]
    public async Task Create_job_works()
    {
        var actions = new JobActions(InvocationContext);

        var result = await actions.CreateJob(new ProjectTypeRequest { ProjectType = "3" }, new CreateJobRequest { ProjectId = "573", ItemId = "406", DueDate = DateTime.Now.AddDays(7), Status = "0" }, new JobTypeRequest { JobType = "TRA" }, new ContactPersonRequest { });
        Assert.IsNotNull(result.ItemId);
    }

    [TestMethod]
    public async Task Assign_resource_to_job_works()
    {
        var actions = new JobActions(InvocationContext);

        var result = await actions.CreateJob(new ProjectTypeRequest { ProjectType = "3" }, new CreateJobRequest { ProjectId = "573", ItemId = "406", DueDate = DateTime.Now.AddDays(7), Status = "0" }, new JobTypeRequest { JobType = "TRA" }, new ContactPersonRequest { });
        var assignResult = await actions.AssignResourceToJob(new AssignResourceRequest { ResourceId = "1", JobId = result.JobId, ProjectType = "3" });
        Assert.IsNotNull(assignResult.ResourceId = "1");
    }

    [TestMethod]
    public async Task Get_Item_Jobs_IsSuccess()
    {
        var actions = new JobActions(InvocationContext);

        var result = await actions.GetItemJobs
            (new ProjectTypeRequest { ProjectType = "3" }, 
            new GetItemRequest { ItemId= "566548" },
            new OptionalJobStatusRequest { }, 
            new JobTypeOptionRequest { });

        foreach (var job in result.Jobs)
        {
            Console.WriteLine(job.JobType);
            Assert.IsNotNull(result);
        }
       
    }
}
