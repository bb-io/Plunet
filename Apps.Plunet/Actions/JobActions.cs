using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Item;
using Apps.Plunet.Models.Job;
using Apps.Plunet.Models.Resource.Request;
using Apps.Plunet.Models.Resource.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataJob30Service;
using DateResult = Blackbird.Plugins.Plunet.DataJob30Service.DateResult;
using IntegerResult = Blackbird.Plugins.Plunet.DataJob30Service.IntegerResult;
using StringResult = Blackbird.Plugins.Plunet.DataJob30Service.StringResult;

namespace Apps.Plunet.Actions
{
    [ActionList]
    public class JobActions : PlunetInvocable
    {
        public JobActions(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        [Action("Get item jobs", Description = "Get all jobs related to a Plunet item")]
        public async Task<ItemJobsResponse> GetItemJobs([ActionParameter] ProjectTypeRequest project, [ActionParameter] GetItemRequest request, [ActionParameter] OptionalJobStatusRequest status)
        {
            var result = status.Status == null ? await ItemClient.getJobsAsync(Uuid, ParseId(project.ProjectType), ParseId(request.ItemId)) :
                await ItemClient.getJobsWithStatusAsync(Uuid, ParseId(status.Status), ParseId(project.ProjectType), ParseId(request.ItemId));

            if (result.statusMessage != ApiResponses.Ok)
                throw new(result.statusMessage);

            var jobs = new List<JobResponse>();

            foreach (var id in result.data)
            {
                var job = await GetJob(new GetJobRequest { JobId = id.ToString(), ProjectType = project.ProjectType });
                jobs.Add(job);
            }

            return new ItemJobsResponse
            {
                Jobs = jobs
            };
        }

        [Action("Get Job", Description = "Get details for a Plunet job")]
        public async Task<JobResponse> GetJob([ActionParameter] GetJobRequest request)
        {
            var type = ParseId(request.ProjectType);
            var id = ParseId(request.JobId);

            var comment = await GetString(JobClient.getCommentAsync(Uuid, type, id));
            var contactPersonId = await GetId(JobClient.getContactPersonIDAsync(Uuid, type, id));
            var creationDate = await GetDate(JobClient.getCreationDateAsync(Uuid, type, id));
            var currency = await GetString(JobClient.getCurrencyAsync(Uuid, id, type));
            var deliveryDate = await GetDate(JobClient.getDeliveryDateAsync(Uuid, type, id));
            var deliveryNote = await GetString(JobClient.getDeliveryNoteAsync(Uuid, type, id));
            var description = await GetString(JobClient.getDescriptionAsync(Uuid, type, id));
            var payableId = await GetId(JobClient.getPayableIDAsync(Uuid, id, type), false);

            var jobViewResponse = await JobClient.getJob_ForViewAsync(Uuid, id, type);

            if (jobViewResponse.statusMessage != ApiResponses.Ok)
                throw new(jobViewResponse.statusMessage);

            var jobMetricsResponse = await JobClient.getJobMetricsAsync(Uuid, id, type, Language);

            if (jobMetricsResponse.statusMessage != ApiResponses.Ok)
                throw new(jobMetricsResponse.statusMessage);

            var jobTrackingResponse = await JobClient.getJobTrackingTimesListAsync(Uuid, id, type);

            if (jobTrackingResponse.statusMessage != ApiResponses.Ok)
                throw new(jobTrackingResponse.statusMessage);

            return new JobResponse
            {
                Comment = comment,
                ContactPersonId = contactPersonId,
                CreationDate = creationDate,
                Currency = currency,
                DeliveryDate = deliveryDate,
                DeliveryNote = deliveryNote,
                Description = description,
                PayableId = payableId,
                NumberOfSourceFiles = jobViewResponse.data.countSourceFiles,
                DueDate = jobViewResponse.data.dueDateSpecified ? jobViewResponse.data.dueDate : null,
                ItemId = jobViewResponse.data.itemID.ToString(),
                JobId = jobViewResponse.data.jobID.ToString(),
                ResourceId = jobViewResponse.data.resourceID.ToString(),
                JobType = jobViewResponse.data.jobTypeFull,
                StartDate = jobViewResponse.data.startDateSpecified ? jobViewResponse.data.startDate : null,
                Status = jobViewResponse.data.status.ToString(),
                TotalPrice = jobMetricsResponse.data.totalPrice,
                PercentageComplated = jobTrackingResponse.data.completed,
            };
        }

        [Action("Delete job", Description = "Delete a Plunet job")]
        public async Task DeleteJob([ActionParameter] GetJobRequest request)
        {
            await JobClient.deleteJobAsync(Uuid, ParseId(request.JobId), ParseId(request.ProjectType));
        }

        [Action("Create job", Description = "Create a new job in Plunet")]
        public async Task<JobResponse> CreateJob([ActionParameter] ProjectTypeRequest project, [ActionParameter] CreateJobRequest input, [ActionParameter] JobTypeRequest type, [ActionParameter] ContactPersonRequest contactPerson)
        {
            var jobIn = new JobIN()
            {
                dueDateSpecified = input.DueDate.HasValue,
                startDateSpecified = input.StartDate.HasValue,
                itemID = ParseId(input.ItemId),
                projectType = ParseId(project.ProjectType),
                status = ParseId(input.Status),
                projectID = ParseId(input.ProjectId),
            };

            if (input.DueDate.HasValue)
                jobIn.dueDate = input.DueDate.Value;

            if (input.StartDate.HasValue)
                jobIn.startDate = input.StartDate.Value;

            var response = await JobClient.insert3Async(Uuid, jobIn, type.JobType);

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            string jobId = response.data.ToString();
            if (contactPerson.ResourceId is not null)
            {
                var result = await JobClient.setContactPersonIDAsync(Uuid, ParseId(project.ProjectType), ParseId(jobId), ParseId(contactPerson.ResourceId));
                
                if (result.statusMessage != ApiResponses.Ok)
                    throw new Exception(result.statusMessage);
            }

            return await GetJob(new GetJobRequest { ProjectType = project.ProjectType, JobId = jobId});
        }

        [Action("Assign resource to job", Description = "Assign a resource to a Plunet job")]
        public async Task<AssignResourceResponse> AssignResourceToJob([ActionParameter] GetJobRequest request, [ActionParameter] ResourceRequest resource)
        {
            var result = await JobClient.setResourceIDAsync(Uuid, ParseId(request.ProjectType), ParseId(resource.ResourceId), ParseId(request.JobId));

            if (result.statusMessage != ApiResponses.Ok)
                throw new(result.statusMessage);

            var jobResource = await JobClient.getResourceIDAsync(Uuid, ParseId(request.ProjectType), ParseId(request.JobId));

            if (jobResource.statusMessage != ApiResponses.Ok)
                throw new(jobResource.statusMessage);

            return new AssignResourceResponse { ResourceId = jobResource.data.ToString() };
        }

        [Action("Update job", Description = "Update an existing job in Plunet")]
        public async Task<JobResponse> UpdateJob([ActionParameter] GetJobRequest request, [ActionParameter] CreateJobRequest input)
        {
            var jobIn = new JobIN()
            {
                dueDateSpecified = input.DueDate.HasValue,
                startDateSpecified = input.StartDate.HasValue,
                itemID = ParseId(input.ItemId),
                jobID = ParseId(request.JobId),
                projectType = ParseId(request.ProjectType),
                status = ParseId(input.Status),
                projectID = ParseId(input.ProjectId)
            };

            if (input.DueDate.HasValue)
                jobIn.dueDate = input.DueDate.Value;

            if (input.StartDate.HasValue)
                jobIn.startDate = input.StartDate.Value;

            var response = await JobClient.updateAsync(Uuid, jobIn, false);

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            return await GetJob(request);
        }

        [Action("Get job pricelines", Description = "Get a list of all pricelines attached to a job")]
        public async Task<PricelinesResponse> GetJobPricelines([ActionParameter] GetJobRequest job)
        {
            var response = await JobClient.getPriceLine_ListAsync(Uuid, ParseId(job.JobId), ParseId(job.ProjectType));

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            return new PricelinesResponse
            {
                Pricelines = response.data.Select(CreatePricelineResponse),
            };
        }

        [Action("Create job priceline", Description = "Add a new pricline to a job")]
        public async Task<PricelineResponse> CreateJobPriceline([ActionParameter] GetJobRequest job, [ActionParameter] JobPriceUnitRequest unit, [ActionParameter] PricelineRequest input)
        {
            var pricelineIn = new Blackbird.Plugins.Plunet.DataJob30Service.PriceLineIN
            {
                amount = input.Amount,
                unit_price = input.UnitPrice,
                memo = input.Memo ?? string.Empty,
                priceUnitID = ParseId(unit.PriceUnit),
            };

            if (input.AmountPerUnit.HasValue)
                pricelineIn.amount_perUnit = input.AmountPerUnit.Value;

            if (input.TimePerUnit.HasValue)
                pricelineIn.time_perUnit = input.TimePerUnit.Value;

            var response = await JobClient.insertPriceLineAsync(Uuid, ParseId(job.JobId), ParseId(job.ProjectType), pricelineIn, false);

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            return CreatePricelineResponse(response.data);
        }

        [Action("Delete job priceline", Description = "Delete a priceline from a job")]
        public async Task DeletePriceline([ActionParameter] GetJobRequest job, [ActionParameter] PricelineIdRequest line)
        {
            var response = await JobClient.deletePriceLineAsync(Uuid, ParseId(job.JobId), ParseId(job.ProjectType), ParseId(line.Id));

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);
        }

        [Action("Update job priceline", Description = "Update an existing job pricline")]
        public async Task<PricelineResponse> UpdateJobPriceline([ActionParameter] GetJobRequest job, [ActionParameter] JobPriceUnitRequest unit, [ActionParameter] PricelineIdRequest line, [ActionParameter] PricelineRequest input)
        {
            var pricelineIn = new Blackbird.Plugins.Plunet.DataJob30Service.PriceLineIN
            {
                amount = input.Amount,
                unit_price = input.UnitPrice,
                memo = input.Memo ?? string.Empty,
                priceUnitID = ParseId(unit.PriceUnit),
                priceLineID = ParseId(line.Id),
            };

            if (input.AmountPerUnit.HasValue)
                pricelineIn.amount_perUnit = input.AmountPerUnit.Value;

            if (input.TimePerUnit.HasValue)
                pricelineIn.time_perUnit = input.TimePerUnit.Value;

            var response = await JobClient.updatePriceLineAsync(Uuid, ParseId(job.JobId), ParseId(job.ProjectType), pricelineIn);

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            return CreatePricelineResponse(response.data);
        }

        private PricelineResponse CreatePricelineResponse(Blackbird.Plugins.Plunet.DataJob30Service.PriceLine line)
        {
            return new PricelineResponse
            {
                Amount = line.amount,
                AmountPerUnit = line.amount_perUnit,
                Memo = line.memo,
                Id = line.priceLineID.ToString(),
                UnitPrice = line.unit_price,
                Sequence = line.sequence,
                TaxType = line.taxType.ToString(),
                TimePerUnit = line.time_perUnit,
                PriceUnitId = line.priceUnitID.ToString(),
            };
        }

        private async Task<string> GetString(Task<StringResult> task)
        {
            var response = await task;
            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);
            return response.data;
        }

        private async Task<string?> GetId(Task<IntegerResult> task, bool throwOnError = true)
        {
            var response = await task;
            if (response.data == 0) return null;
            if (response.statusMessage != ApiResponses.Ok)
            {
                if (throwOnError)
                    throw new(response.statusMessage);
                return null;
            }
                
            return response.data.ToString();
        }

        private async Task<DateTime> GetDate(Task<DateResult> task)
        {
            var response = await task;
            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);
            return response.data;
        }

        // Item independent jobs?
        // Start automatic job?

    }
}
