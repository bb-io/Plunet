using Apps.Plunet.Actions;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Item;
using Apps.Plunet.Models.Job;
using Apps.Plunet.Models.Request.Request;
using Newtonsoft.Json;
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
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
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

    [TestMethod]
    public async Task Create_job_priceline_works()
    {
        var actions = new JobActions(InvocationContext);

        var result = await actions.CreateJobPriceline(new GetJobRequest {ProjectType= "3", JobId= "189" }, new JobPriceUnitRequest {
            PriceUnit = null, Service = "Translation"
        },
            new PricelineRequest { Amount = 155, UnitPrice = 155, TaxType = "8" });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task StartJobAssigmentRound_ReturnsJobRoundDto()
    {
        // Arrange
        var action = new JobActions(InvocationContext);
        var job = new GetJobRequest { ProjectType = "3", JobId = "213" };
        string status = "8";

        // Act 
        var result = await action.UpdateJobRoundStatus(job, status);

        // Assert
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetJobRounds_ReturnsJobRounds()
    {
        // Arrange
        var action = new JobActions(InvocationContext);
        var job = new GetJobRequest { ProjectType = "3", JobId = "214" };

        // Act
        var result = await action.GetJobRounds(job);

        // Assert
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task StartAutomaticJob()
    {
        // Arrange
        var actions = new JobActions(InvocationContext);
        var job = new GetJobRequest { ProjectType = "3", JobId = "262" };

        // Act
        await actions.StartAutomaticJob(job);
    }
}
