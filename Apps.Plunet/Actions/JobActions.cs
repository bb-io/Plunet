using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.CustomProperties;
using Apps.Plunet.Models.Item;
using Apps.Plunet.Models.Job;
using Apps.Plunet.Models.Request.Request;
using Apps.Plunet.Models.Resource.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataJob30Service;
using Blackbird.Plugins.Plunet.DataResource30Service;
using DataJobRound30Service;

namespace Apps.Plunet.Actions;

[ActionList("Jobs")]
public class JobActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{
    [Action("Get item jobs", Description = "Get all jobs related to a Plunet item")]
    public async Task<ItemJobsResponse> GetItemJobs([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest request, [ActionParameter] OptionalJobStatusRequest status,
        [ActionParameter] JobTypeOptionRequest? jobType)
    {
        var result = status.Status == null
            ? await ExecuteWithRetry(() => ItemClient.getJobsAsync(Uuid, ParseId(project.ProjectType), ParseId(request.ItemId)))
            : await ExecuteWithRetry(() => ItemClient.getJobsWithStatusAsync(Uuid, ParseId(status.Status), ParseId(project.ProjectType), ParseId(request.ItemId)));

        var jobs = new List<JobResponse>();

        if (result != null)
        {
            foreach (var id in result.Where(x => x.HasValue).Select(x => x.Value))
            {
                var job = await GetJob(new GetJobRequest { JobId = id.ToString(), ProjectType = project.ProjectType });
                jobs.Add(job);
            }
        }

        if (!string.IsNullOrWhiteSpace(jobType?.JobType))
        {
            jobs = jobs.Where(j => j.JobType == jobType.JobType).ToList();
        }

        return new ItemJobsResponse
        {
            Jobs = jobs
        };
    }

    [Action("Get item independent jobs", Description = "Get all item-independent jobs of a project")]
    public async Task<ItemJobsResponse> GetItemIndependentJobs(
    [ActionParameter] ProjectTypeRequest project,
    [ActionParameter][Display("Project ID")] string ProjectId,
    [ActionParameter] JobTypeOptionRequest? jobType)
    {
        var result = await ExecuteWithRetry(() =>
            JobClient.getItemIndependentJobsAsync(
                Uuid,
                ParseId(project.ProjectType),
                ParseId(ProjectId)));

        var jobs = new List<JobResponse>();

        if (result != null)
        {
            foreach (var id in result.Select(x => x.jobID))
            {
                var job = await GetJob(new GetJobRequest
                {
                    JobId = id.ToString(),
                    ProjectType = project.ProjectType
                });

                jobs.Add(job);
            }
        }

        if (!string.IsNullOrWhiteSpace(jobType?.JobType))
        {
            jobs = jobs.Where(j => j.JobType == jobType.JobType).ToList();
        }

        return new ItemJobsResponse
        {
            Jobs = jobs
        };
    }


    [Action("Get item job by text module", Description = "Get all jobs related to a Plunet item")]
    public async Task<JobResponse> GetItemJobByTextModule([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest request, [ActionParameter] OptionalJobStatusRequest status,
        [ActionParameter] FindJobByTextModule textModuleRequest)
    {
        var itemJobsResponse = await GetItemJobs(project, request, status,null);

        if (textModuleRequest.Flag is not null)
        {
            foreach (var job in itemJobsResponse.Jobs)
            {
                var textModule = await ExecuteWithRetryAcceptNull(() => CustomFieldsClient.getTextmoduleAsync(Uuid, textModuleRequest.Flag, ParseId(textModuleRequest.UsageArea), ParseId(job.JobId), Language));
                if (textModule?.stringValue == textModuleRequest.TextModuleValue)
                {
                    return job;
                }
            }

            throw new PluginMisconfigurationException($"Given the flag {textModuleRequest.Flag}, no jobs were found.");
        }

        if (!itemJobsResponse.Jobs.Any())
            throw new PluginMisconfigurationException("No jobs were found.");

        return itemJobsResponse.Jobs.First();
    }

    [Action("Get Job", Description = "Get details for a Plunet job")]
    public async Task<JobResponse> GetJob([ActionParameter] GetJobRequest request)
    {
        var type = ParseId(request.ProjectType);
        var id = ParseId(request.JobId);

        var comment = await ExecuteWithRetry(() => JobClient.getCommentAsync(Uuid, type, id));
        var contactPersonId = await ExecuteWithRetryAcceptNull(() => JobClient.getContactPersonIDAsync(Uuid, type, id));
        var creationDate = await ExecuteWithRetry(() => JobClient.getCreationDateAsync(Uuid, type, id));
        var currency = await ExecuteWithRetry(() => JobClient.getCurrencyAsync(Uuid, id, type));
        var deliveryDate = await ExecuteWithRetry(() => JobClient.getDeliveryDateAsync(Uuid, type, id));
        var deliveryNote = await ExecuteWithRetry(() => JobClient.getDeliveryNoteAsync(Uuid, type, id));
        var description = await ExecuteWithRetry(() => JobClient.getDescriptionAsync(Uuid, type, id));
        var payableId = await ExecuteWithRetryAcceptNull(() => JobClient.getPayableIDAsync(Uuid, id, type));

        var jobViewResponse = await ExecuteWithRetry(() => JobClient.getJob_ForViewAsync(Uuid, id, type));

        var jobMetricsResponse = await ExecuteWithRetry(() => JobClient.getJobMetricsAsync(Uuid, id, type, Language));

        var jobTrackingResponse = await ExecuteWithRetry(() => JobClient.getJobTrackingTimesListAsync(Uuid, id, type));

        return new JobResponse
        {
            Comment = comment,
            ContactPersonId = contactPersonId?.ToString(),
            CreationDate = creationDate,
            Currency = currency,
            DeliveryDate = deliveryDate,
            DeliveryNote = deliveryNote,
            Description = description,
            PayableId = payableId?.ToString(),
            NumberOfSourceFiles = jobViewResponse.countSourceFiles,
            DueDate = jobViewResponse.dueDateSpecified ? jobViewResponse.dueDate : null,
            ItemId = jobViewResponse.itemID.ToString(),
            JobId = jobViewResponse.jobID.ToString(),
            ResourceId = jobViewResponse.resourceID.ToString(),
            JobType = jobViewResponse.jobTypeFull,
            StartDate = jobViewResponse.startDateSpecified ? jobViewResponse.startDate : null,
            Status = jobViewResponse.status.ToString(),
            TotalPrice = jobMetricsResponse.totalPrice,
            PercentageComplated = jobTrackingResponse.completed,
        };
    }

    [Action("Delete job", Description = "Delete a Plunet job")]
    public async Task DeleteJob([ActionParameter] GetJobRequest request)
    {
        await ExecuteWithRetry(() => JobClient.deleteJobAsync(Uuid, ParseId(request.JobId), ParseId(request.ProjectType)));
    }

    [Action("Create job", Description = "Create a new job in Plunet")]
    public async Task<JobResponse> CreateJob([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] CreateJobRequest input, [ActionParameter] JobTypeRequest type,
        [ActionParameter] ContactPersonRequest contactPerson)
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

        var response = await ExecuteWithRetry(() => JobClient.insert3Async(Uuid, jobIn, type.JobType));

        string jobId = response.ToString();
        if (contactPerson.ResourceId is not null)
        {
            await ExecuteWithRetry(() => JobClient.setContactPersonIDAsync(Uuid, ParseId(project.ProjectType), ParseId(jobId), ParseId(contactPerson.ResourceId)));
        }

        return await GetJob(new GetJobRequest { ProjectType = project.ProjectType, JobId = jobId });
    }

    [Action("Assign resource to job", Description = "Assign a resource to a Plunet job")]
    public async Task<AssignResourceResponse> AssignResourceToJob([ActionParameter] AssignResourceRequest input)
    {
        var roundId = input.RoundId;

        if (string.IsNullOrEmpty(roundId))
        {
            var roundResponse = await ExecuteWithRetry(() => JobRoundClient.getAllRoundIDsAsync(Uuid, ParseId(input.JobId), ParseId(input.ProjectType)));
            if (roundResponse is null || !roundResponse.LastOrDefault().HasValue) throw new PluginMisconfigurationException("The selected job has no rounds associated");
            roundId = roundResponse.LastOrDefault().ToString();
        }

        await ExecuteWithRetry(() => JobRoundClient.assignResourceAsync(Uuid, ParseId(input.ResourceId), ParseId(input.ResourceContactId), ParseId(roundId)));

        var jobResource = await ExecuteWithRetry(() => JobClient.getResourceIDAsync(Uuid, ParseId(input.ProjectType), ParseId(input.JobId)));

        return new AssignResourceResponse { ResourceId = jobResource.ToString() };
    }

    [Action("Change job round status (experimental)", Description = "Updates status of the first job round.")]
    public async Task<JobRoundDto> UpdateJobRoundStatus(
        [ActionParameter] GetJobRequest job,
        [ActionParameter, StaticDataSource(typeof(JobRoundStatusDataHandler)), Display("New status")] string newStatus)
    {
        var roundIn = new JobRoundIN
        {
            jobRoundNumber = 0,
            startNextRoundAutomatically = true,
            jobID = ParseId(job.JobId),
            projectType = ParseId(job.ProjectType)
        };

        var rounds = await ExecuteWithRetry(() => JobRoundClient.getAllRoundIDsAsync(
            Uuid, 
            ParseId(job.JobId),
            ParseId(job.ProjectType)
        ));
        var round = await ExecuteWithRetry(() => JobRoundClient.getRoundObjectAsync(Uuid, (int)rounds.First()!));

        // Copy everything from a fetched round except for status
        var updatedRoundIn = new JobRoundIN
        {
            jobRoundID = round.jobRoundID,
            jobRoundNumber = round.jobRoundNumber,
            assignmentMethod = round.assignmentMethod,
            assignmentLimitType = round.assignmentLimitType,
            assignmentLimitToFirstX = round.assignmentLimitToFirstX,
            startNextRoundAutomatically = round.startNextRoundAutomatically,
            jobID = round.jobID,
            projectType = round.projectType,
            status = ParseId(newStatus, 0)
        };

        await ExecuteWithRetry(() => JobRoundClient.updateRoundAsync(Uuid, updatedRoundIn));

        var roundAfterUpdate = await ExecuteWithRetry(() => JobRoundClient.getRoundObjectAsync(Uuid, (int)rounds.First()!));
        return new JobRoundDto
        {
            Id = roundAfterUpdate.jobRoundID.ToString(),
            Number = roundAfterUpdate.jobRoundNumber,
            assignmentLimitToFirstX = roundAfterUpdate.assignmentLimitToFirstX.ToString(),
            assignmentLimitType = MapLimitType(roundAfterUpdate.assignmentLimitType),
            assignmentMethod = MapAssignmentMethod(roundAfterUpdate.assignmentMethod),
            Status = roundAfterUpdate.status
        };
    }

    [Action("Start automatic job", Description = "Starts an automatic job")]
    public async Task StartAutomaticJob([ActionParameter] GetJobRequest job)
    {
        await ExecuteWithRetry(async () => 
            await JobClient.runAutomaticJobAsync(Uuid, ParseId(job.JobId), ParseId(job.ProjectType))
        );
    }

    //[Action("Select resources for a job round", Description = "Selects resources for a job round")]
    public async Task SelectJobRoundResources(
        [ActionParameter] GetJobRequest jobRequest,
        [ActionParameter] JobRoundRequest roundRequest,
        [ActionParameter] SelectJobRoundResourceRequest input)
    {
        var jobRoundResources = await ExecuteWithRetry(async () =>
            await JobRoundClient.getResourcesForRoundAsync(Uuid, ParseId(roundRequest.JobRoundId))
        );

        var resourcesToInclude = jobRoundResources.ToList();
        if (input.ResourceIdsToExclude is not null && input.ResourceIdsToExclude.Count > 0)
        {
            var resourcesToExclude = input.ResourceIdsToExclude
                .Select(ParseId)
                .ToHashSet();

            resourcesToInclude = resourcesToInclude
                .Where(r => !resourcesToExclude.Contains((int)r!))
                .ToList();
        }
        resourcesToInclude = resourcesToInclude.Take(input.NumberOfResources).ToList();

        var round = await ExecuteWithRetry(async () => await JobRoundClient.getRoundObjectAsync(
            Uuid, 
            ParseId(roundRequest.JobRoundId))
        );

        var roundIn = new JobRoundIN
        {
            jobRoundID = round.jobRoundID,
            jobID = round.jobID,
            assignmentMethod = 5,
            assignmentLimitType = 2,
            projectType = ParseId(jobRequest.ProjectType)
        };

        await ExecuteWithRetry(async () => await JobRoundClient.updateRoundAsync(Uuid, roundIn));

        foreach (var resource in resourcesToInclude)
        {
            int resourceContactId = await ExecuteWithRetry(async () => await JobClient.getContactPersonIDAsync(
                Uuid,
                ParseId(jobRequest.ProjectType),
                ParseId(jobRequest.JobId))
            );

            await ExecuteWithRetry(async () => await JobRoundClient.assignResourceAsync(
                Uuid,
                (int)resource!,
                resourceContactId,
                ParseId(roundRequest.JobRoundId))
            );
        }
    }

    [Action("Update job", Description = "Update an existing job in Plunet")]
    public async Task<JobResponse> UpdateJob([ActionParameter] GetJobRequest request,
        [ActionParameter] CreateJobRequest input, [ActionParameter] [Display("Pricelist ID")]string? pricelist)
    {
        var jobId = ParseId(request.JobId);
        var projectType = ParseId(request.ProjectType);

        var jobIn = new JobIN
        {
            dueDateSpecified = input.DueDate.HasValue,
            startDateSpecified = input.StartDate.HasValue,
            itemID = ParseId(input.ItemId),
            jobID = jobId,
            projectType = projectType,
            status = ParseId(input.Status),
            projectID = ParseId(input.ProjectId)
        };

        if (input.DueDate.HasValue)
            jobIn.dueDate = input.DueDate.Value;

        if (input.StartDate.HasValue)
            jobIn.startDate = input.StartDate.Value;

        await ExecuteWithRetry(() => JobClient.updateAsync(Uuid, jobIn, false));

        if (!String.IsNullOrEmpty(pricelist))
        {
            await ExecuteWithRetry(() => JobClient.setPricelistAsync(Uuid, jobId, projectType, ParseId(pricelist)));
        }

        return await GetJob(request);
    }

    [Action("Get job pricelines", Description = "Get a list of all pricelines attached to a job")]
    public async Task<PricelinesResponse> GetJobPricelines([ActionParameter] GetJobRequest job)
    {
        var response = await ExecuteWithRetryAcceptNull(() => JobClient.getPriceLine_ListAsync(Uuid, ParseId(job.JobId), ParseId(job.ProjectType)));

        if (response is null)
        {
            return new PricelinesResponse();
        }
        
        var result = new List<PricelineResponse>();

        foreach (var priceLine in response)
        {
            var priceUnit = await ExecuteWithRetry(() => JobClient.getPriceUnitAsync(Uuid, priceLine.priceUnitID, Language));
            result.Add(CreatePricelineResponse(priceLine, priceUnit));
        }

        return new PricelinesResponse
        {
            Pricelines = result,
        };
    }

    [Action("Create job priceline", Description = "Add a new pricline to a job")]
    public async Task<PricelineResponse> CreateJobPriceline([ActionParameter] GetJobRequest job,
        [ActionParameter] JobPriceUnitRequest unit, [ActionParameter] PricelineRequest input)
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

        if (!string.IsNullOrEmpty(input.TaxType))
            pricelineIn.taxType = int.Parse(input.TaxType);

        var response = await ExecuteWithRetryAcceptNull(() => JobClient.insertPriceLineAsync(Uuid, ParseId(job.JobId), ParseId(job.ProjectType), pricelineIn, false));

        if (response is null)
        {
            return new PricelineResponse();
        }

        var priceUnit = await ExecuteWithRetry(() => JobClient.getPriceUnitAsync(Uuid, response.priceUnitID, Language));
        return CreatePricelineResponse(response, priceUnit);
    }

    [Action("Delete job priceline", Description = "Delete a priceline from a job")]
    public async Task DeletePriceline([ActionParameter] GetJobRequest job, [ActionParameter] PricelineIdRequest line)
    {
        await ExecuteWithRetry(() => JobClient.deletePriceLineAsync(Uuid, ParseId(job.JobId), ParseId(job.ProjectType), ParseId(line.Id)));
    }

    [Action("Update job priceline", Description = "Update an existing job pricline")]
    public async Task<PricelineResponse> UpdateJobPriceline([ActionParameter] GetJobRequest job,
        [ActionParameter] JobPriceUnitRequest unit, [ActionParameter] PricelineIdRequest line,
        [ActionParameter] PricelineRequest input)
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

        if (!string.IsNullOrEmpty(input.TaxType))
            pricelineIn.taxType = int.Parse(input.TaxType);

        var response = await ExecuteWithRetryAcceptNull(() => JobClient.updatePriceLineAsync(Uuid, ParseId(job.JobId), ParseId(job.ProjectType), pricelineIn));

        if (response is null)
        {
            return new PricelineResponse();
        }

        var priceUnit = await ExecuteWithRetry(() => JobClient.getPriceUnitAsync(Uuid, response.priceUnitID, Language));
        return CreatePricelineResponse(response, priceUnit);
        
    }

    [Action("Get job rounds", Description = "Get all rounds for a specific job")]
    public async Task<JobRoundsResponse> GetJobRounds([ActionParameter] GetJobRequest job)
    {
        var roundResponse = await ExecuteWithRetry(() => JobRoundClient.getAllRoundIDsAsync(Uuid, ParseId(job.JobId), ParseId(job.ProjectType)));
        var result = new JobRoundsResponse
        {
            JobRounds = new List<JobRoundDto>()
        };

        if (roundResponse.Length > 0)
        {
            var assignedRoundId = await ExecuteWithRetry(() => JobRoundClient.getAssignedRoundIDAsync(Uuid, ParseId(job.JobId), ParseId(job.ProjectType)));

            foreach (var round in roundResponse)
            {
                var response = await ExecuteWithRetry(() => JobRoundClient.getRoundObjectAsync(Uuid, round.Value));

                var roundDto = new JobRoundDto
                {
                    Id = response.jobRoundID.ToString(),
                    Number = response.jobRoundNumber,
                    assignmentLimitToFirstX = response.assignmentLimitToFirstX.ToString(),
                    assignmentLimitType = MapLimitType(response.assignmentLimitType),
                    assignmentMethod = MapAssignmentMethod(response.assignmentMethod),
                    Assigned = response.jobRoundID == assignedRoundId
                };

                result.JobRounds.Add(roundDto);
            }
        }

        return result;
    }

    [Action("Get job feedback", Description = "Get the feedback for a specific job")]
    public async Task<JobFeedbackResponse> GetJobFeedback([ActionParameter] GetJobRequest job)
    {
        var feedback = await ExecuteWithRetry(() => QualityManagerClient.getJobFeedbackAsync(Uuid, ParseId(job.JobId)));

        var ScoreList = new List<qualityScore>();
        if (feedback.ratingList.Count() > 0)
        {
            var criteria = await AdminClient.getJobFeedbackCriteriaAsync(Uuid);

            foreach (var item in feedback.ratingList)
            {
                var subRating = new qualityScore
                {
                    Name = criteria.data.FirstOrDefault(x => x.id == item.criterionID) is null ? "" :
                    criteria.data.FirstOrDefault(x => x.id == item.criterionID)!.label,
                    Critical = item.lisaRating.amount_critical,
                    Hard = item.lisaRating.amount_hard,
                    Minor = item.lisaRating.amount_minor,
                    Rating = item.rating                
                };
                ScoreList.Add(subRating);
            }
        }        

        var result = new JobFeedbackResponse
        {
            EditorUserID = feedback.editorUserID,
            ModifiedAt = feedback.modifiedAt,
            Rating = feedback.rating,
            IsJobQualityRatingClosed = feedback.jobQualityRatingClosed,
            Commentary = feedback.commentary,
            JobID = feedback.jobID.ToString(),
            SubScores = ScoreList
            
        };
                
        return result;
    }

    private PricelineResponse CreatePricelineResponse(PriceLine line, PriceUnit? unit)
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
            PriceUnitDescription = unit?.description ?? "",
            PriceUnitService = unit?.service ?? ""
        };
    }

    private string MapAssignmentMethod(int method)
    {
        return method switch
        {
            1 => "Manual",
            2 => "First come first serve",
            4 => "Top ranked",
            5 => "Manual direct assignment",
            _ => method.ToString() 
        };
    }

    private string MapLimitType(int type)
    {
        return type switch
        {
            0 => "All",
            1 => "Limit X",
            2 => "Manual selected",
            _ => type.ToString()
        };
    }

    // Start automatic job?
}