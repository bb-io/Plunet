using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Item;
using Apps.Plunet.Models.Job;
using Apps.Plunet.Models.Request.Request;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataItem30Service;
using PriceUnit = Blackbird.Plugins.Plunet.DataItem30Service.PriceUnit;

namespace Apps.Plunet.Actions;

[ActionList("Items")]
public class ItemActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{
    [Action("Search items",
        Description = "Search for items either under a specific order/quote or with a certain status")]
    public async Task<SearchResponse<ItemResponse>> SearchItems(
        [ActionParameter] OptionalItemProjectRequest item,
        [ActionParameter] SearchItemsRequest searchParams,
        [ActionParameter] OptionalCurrencyTypeRequest currencyParams,
        [ActionParameter] OptionalTargetLanguageRequest targetLanguage)
    {
        IEnumerable<Item>? result;

        if (item.ProjectId == null)
        {
            if (searchParams.Status == null || !searchParams.Status.Any())
                throw new PluginMisconfigurationException("Please provide either an order or quote ID or at least one item status.");

            if (searchParams.DocumentStatus == null)
            {
                result = await FetchItemsByStatusesAsync(searchParams.Status, async status =>
                    await ExecuteWithRetryAcceptNull(() => ItemClient.getItemsByStatus1Async(Uuid, ParseId(item.ProjectType), ParseId(status))
                ));
            }
            else
            {
                result = await FetchItemsByStatusesAsync(searchParams.Status, async status => currencyParams.CurrencyType == null
                    ? await ExecuteWithRetryAcceptNull(() => ItemClient.getItemsByStatus3Async(
                        Uuid,
                        ParseId(item.ProjectType),
                        ParseId(status),
                        ParseId(searchParams.DocumentStatus))
                    )
                    : await ExecuteWithRetryAcceptNull(() => ItemClient.getItemsByStatus3ByCurrencyTypeAsync(
                        Uuid,
                        ParseId(item.ProjectType),
                        ParseId(status),
                        ParseId(searchParams.DocumentStatus),
                        ParseId(currencyParams.CurrencyType))
                    )
                );
            }
        }
        else
        {
            if (searchParams.Status == null)
            {
                result = currencyParams.CurrencyType == null
                    ? await ExecuteWithRetryAcceptNull(() => ItemClient.getAllItemObjectsAsync(
                        Uuid,
                        ParseId(item.ProjectId),
                        ParseId(item.ProjectType))
                    )
                    : await ExecuteWithRetryAcceptNull(() => ItemClient.getAllItemObjectsByCurrencyAsync(
                        Uuid,
                        ParseId(item.ProjectId),
                        ParseId(item.ProjectType),
                        ParseId(currencyParams.CurrencyType))
                    );
            }
            else if (searchParams.DocumentStatus == null)
            {
                result = await FetchItemsByStatusesAsync(searchParams.Status, async status =>
                    await ExecuteWithRetryAcceptNull(() => ItemClient.getItemsByStatus2Async(
                        Uuid,
                        ParseId(item.ProjectId),
                        ParseId(item.ProjectType),
                        ParseId(status))
                    )
                );
            }
            else
            {
                result = await FetchItemsByStatusesAsync(searchParams.Status, async status => currencyParams.CurrencyType == null
                    ? await ExecuteWithRetryAcceptNull(() => ItemClient.getItemsByStatus4Async(
                        Uuid,
                        ParseId(item.ProjectId),
                        ParseId(item.ProjectType),
                        ParseId(status),
                        ParseId(searchParams.DocumentStatus))
                    )
                    : await ExecuteWithRetryAcceptNull(() => ItemClient.getItemsByStatus4ByCurrencyTypeAsync(
                        Uuid,
                        ParseId(item.ProjectId),
                        ParseId(item.ProjectType),
                        ParseId(status),
                        ParseId(searchParams.DocumentStatus),
                        ParseId(currencyParams.CurrencyType))
                    )
                );
            }
        }

        var projectType = (ItemProjectType)int.Parse(item.ProjectType);
        if (targetLanguage != null && !string.IsNullOrEmpty(targetLanguage.TargetLanguageName))
        {
            result = result?.Where(x => x.targetLanguage == targetLanguage.TargetLanguageName).ToArray();
        }
        var items = result is null || !result.Any()
            ? new List<ItemResponse>()
            : result.Take(searchParams.Limit ?? SystemConsts.SearchLimit).Select(x => new ItemResponse(x, projectType)).ToList();

        foreach (var fetchedItem in items)
        {
            await GetDeliveryDate(fetchedItem);
        }

        return new SearchResponse<ItemResponse>(items);
    }

    private async Task<IEnumerable<Item>> FetchItemsByStatusesAsync(
        IEnumerable<string> statuses,
        Func<string, Task<Item[]?>> fetchFunc)
    {
        var aggregated = new List<Item>();
        foreach (var status in statuses)
        {
            var items = await fetchFunc(status);
            if (items != null)
                aggregated.AddRange(items);
        }
        return aggregated;
    }

    [Action("Find item", Description = "Find a specific item based on specific criteria")]
    public async Task<FindResponse<ItemResponse>> FindItem([ActionParameter] OptionalItemProjectRequest item,
        [ActionParameter] SearchItemsRequest searchParams,
        [ActionParameter] OptionalCurrencyTypeRequest currencyParams,
        [ActionParameter] OptionalTargetLanguageRequest targetLanguage)
    {
        var result = await SearchItems(item, searchParams, currencyParams, targetLanguage);
        return new(result.Items.FirstOrDefault(), result.TotalCount);
    }

    [Action("Get language independent item", Description = "Get details for a language independent item in a project")]
    public async Task<ItemResponse> GetLanguageIndependentItem([ActionParameter] ProjectTypeRequest projectType,
        [ActionParameter] ProjectIdRequest projectId,
        [ActionParameter] CurrencyTypeRequest currencyParams)
    {
        var result = await ExecuteWithRetry(() => ItemClient.getLanguageIndependentItemObjectAsync(Uuid,
                        ParseId(projectType.ProjectType), ParseId(projectId.ProjectId), ParseId(currencyParams.CurrencyType)));
        var Type = (ItemProjectType)int.Parse(projectType.ProjectType);

        var item = new ItemResponse(result, Type);
        await GetDeliveryDate(item);
        return item;
    }

    [Action("Get item", Description = "Get details for a Plunet item")]
    public async Task<ItemResponse> GetItem([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest request, [ActionParameter] OptionalCurrencyTypeRequest currency)
    {
        var result = currency.CurrencyType == null
            ? await ExecuteWithRetry(() => ItemClient.getItemObjectAsync(Uuid, ParseId(project.ProjectType), ParseId(request.ItemId)))
            : await ExecuteWithRetry(() => ItemClient.getItemObjectByCurrencyTypeAsync(Uuid, ParseId(project.ProjectType), ParseId(request.ItemId), ParseId(currency.CurrencyType)));

        var projectType = (ItemProjectType)int.Parse(project.ProjectType);
        var item = new ItemResponse(result, projectType);
        await GetDeliveryDate(item);
        return item;
    }

    private async Task GetDeliveryDate(ItemResponse item)
    {
        try
        {
            var date = await ExecuteWithRetry(() => ItemClient.getDeliveryDateAsync(Uuid, ParseId(item.ItemId)));
            item.DeliveryDate = date.data == DateTime.MinValue ? null : date.data;
        }
        catch (PluginApplicationException ex) when (ex.Message.Contains("System can't find the requested item", StringComparison.OrdinalIgnoreCase))
        {
            item.DeliveryDate = null;
        }
    }

    [Action("Copy jobs from workflow", Description = "Copy jobs from the selected workflow into the specified item")]
    public async Task<ItemJobsResponse> CopyJobsFromWorkflow([ActionParameter] WorkflowIdRequest workflow,
        [ActionParameter] ProjectTypeRequest project, [ActionParameter] GetItemRequest request)
    {
        var workflowId = ParseId(workflow.WorkflowId);
        var projectType = ParseId(project.ProjectType);
        var itemId = ParseId(request.ItemId);

        await ExecuteWithRetry(() => ItemClient.copyJobsFromWorkflowAsync(Uuid, workflowId, projectType, itemId));

        var jobActions = new JobActions(InvocationContext);
        return await jobActions.GetItemJobs(project, request, new OptionalJobStatusRequest { }, new JobTypeOptionRequest { });
    }

    [Action("Create item", Description = "Create a new item in Plunet")]
    public async Task<ItemResponse> CreateItem(
        [ActionParameter] ProjectTypeRequest project,
        [ActionParameter] ProjectIdRequest projectId,
        [ActionParameter] CreateItemRequest request,
        [ActionParameter][Display("Pricelist ID")] string? pricelist,
        [ActionParameter] OptionalLanguageCombinationRequest languages)
    {
        if ((languages.SourceLanguageCode == null) != (languages.TargetLanguageCode == null))
        {
            throw new PluginMisconfigurationException("Either both source and target languages should be defined or neither");
        }

        var itemIn = new ItemIN()
        {
            briefDescription = request.BriefDescription ?? string.Empty,
            comment = request.Comment ?? string.Empty,
            deliveryDeadline = request.Deadline ?? default,
            projectID = ParseId(projectId.ProjectId),
            projectType = ParseId(project.ProjectType),
            reference = request.Reference ?? string.Empty,
            status = ParseId(request.Status),
            taxType = ParseId(request.TaxType),
            totalPrice = request.TotalPrice ?? default,
        };

        var response = languages.SourceLanguageCode == null
            ? await ExecuteWithRetry(() => ItemClient.insertLanguageIndependentItemAsync(Uuid, itemIn))
            : await ExecuteWithRetry(() => ItemClient.insert2Async(Uuid, itemIn));

        await HandleLanguages(languages, ParseId(project.ProjectType), ParseId(projectId.ProjectId), response);

        if (!String.IsNullOrEmpty(pricelist))
        {
            await ExecuteWithRetry(() => ItemClient.setPricelistAsync(Uuid, response, ParseId(project.ProjectType), ParseId(pricelist)));
        }

        return await GetItem(project, new GetItemRequest { ItemId = response.ToString() }, new OptionalCurrencyTypeRequest { });
    }

    [Action("Delete item", Description = "Delete a Plunet item")]
    public async Task DeleteItem([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest request)
    {
        await ExecuteWithRetry(() => ItemClient.deleteAsync(Uuid, ParseId(request.ItemId), ParseId(project.ProjectType)));
    }

    [Action("Update item", Description = "Update an existing item in Plunet")]
    public async Task<ItemResponse> UpdateItem([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest item, [ActionParameter] CreateItemRequest request,
        [ActionParameter][Display("Pricelist ID")] string? pricelist,
        [ActionParameter] OptionalLanguageCombinationRequest languages)
    {
        if ((languages.SourceLanguageCode == null) != (languages.TargetLanguageCode == null))
        {
            throw new PluginMisconfigurationException("Either both source and target languages should be defined or neither");
        }

        var itemIn = new ItemIN()
        {
            briefDescription = request.BriefDescription ?? string.Empty,
            comment = request.Comment ?? string.Empty,
            deliveryDeadline = request.Deadline ?? default,
            itemID = ParseId(item.ItemId),
            reference = request.Reference ?? string.Empty,
            status = ParseId(request.Status),
            projectType = ParseId(project.ProjectType),
        };

        await ExecuteWithRetry(() => ItemClient.updateAsync(Uuid, itemIn, false));

        if (!String.IsNullOrEmpty(pricelist))
        {
            await ExecuteWithRetry(() => ItemClient.setPricelistAsync(Uuid, ParseId(item.ItemId), ParseId(project.ProjectType),  ParseId(pricelist)));
        }

        var itemRes = await ExecuteWithRetry(() => ItemClient.getItemObjectAsync(Uuid, ParseId(project.ProjectType), ParseId(item.ItemId)));
        await HandleLanguages(languages, ParseId(project.ProjectType), itemRes.projectID, ParseId(item.ItemId));
        return await GetItem(project, item, new OptionalCurrencyTypeRequest { });
    }

    [Action("Get item pricelines", Description = "Get a list of all pricelines attached to an item")]
    public async Task<PricelinesResponse> GetItemPricelines([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest item)
    {
        var response = await ExecuteWithRetryAcceptNull(() => ItemClient.getPriceLine_ListAsync(Uuid, ParseId(item.ItemId), ParseId(project.ProjectType)));

        if (response is null)
        {
            return new PricelinesResponse();
        }

        var result = new List<PricelineResponse>();

        foreach (var priceLine in response)
        {
            var priceUnit = await ExecuteWithRetry(() => ItemClient.getPriceUnitAsync(Uuid, priceLine.priceUnitID, Language));
            result.Add(CreatePricelineResponse(priceLine, priceUnit));
        }

        return new PricelinesResponse
        {
            Pricelines = result,
        };

    }

    [Action("Create item priceline", Description = "Add a new priceline to an item")]
    public async Task<PricelineResponse> CreateItemPriceline([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest item, [ActionParameter] ItemPriceUnitRequest unit,
        [ActionParameter] PricelineRequest input)
    {
        var pricelineIn = new PriceLineIN
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

        var response = await ExecuteWithRetryAcceptNull(() => ItemClient.insertPriceLineAsync(Uuid, ParseId(item.ItemId), ParseId(project.ProjectType), pricelineIn, false));
        if (response == null)
        {
            throw new PluginApplicationException("Failed to insert priceline: API returned null. Please check your input and try again");
        }
        var priceUnit = await ExecuteWithRetry(() => ItemClient.getPriceUnitAsync(Uuid, int.Parse(unit.PriceUnit), Language));

        return CreatePricelineResponse(response, priceUnit);
    }

    [Action("Delete item priceline", Description = "Delete a priceline from an item")]
    public async Task DeletePriceline([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest item, [ActionParameter] PricelineIdRequest line)
    {
        await ExecuteWithRetry(() => ItemClient.deletePriceLineAsync(Uuid, ParseId(item.ItemId), ParseId(project.ProjectType), ParseId(line.Id)));
    }

    [Action("Update item priceline", Description = "Update an existing item priceline")]
    public async Task<PricelineResponse> UpdateItemPriceline([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest item, [ActionParameter] ItemPriceUnitRequest unit,
        [ActionParameter] PricelineIdRequest line, [ActionParameter] PricelineRequest input)
    {
        var pricelineIn = new PriceLineIN
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

        var response = await ExecuteWithRetryAcceptNull(() => ItemClient.updatePriceLineAsync(Uuid, ParseId(item.ItemId), ParseId(project.ProjectType), pricelineIn));

        var priceUnit = await ExecuteWithRetry(() => ItemClient.getPriceUnitAsync(Uuid, int.Parse(unit.PriceUnit), Language));
        return CreatePricelineResponse(response, priceUnit);
    }

    private PricelineResponse CreatePricelineResponse(PriceLine line, PriceUnit? unit)
    {

        return new PricelineResponse
        {
            Amount = line.amount,
            AmountPerUnit = line.amount_perUnit,
            Memo = line.memo ?? string.Empty,
            Id = line.priceLineID.ToString(),
            UnitPrice = line.unit_price,
            Sequence = line.sequence,
            TaxType = line.taxType.ToString() ?? string.Empty,
            TimePerUnit = line.time_perUnit,
            PriceUnitId = line.priceUnitID.ToString(),
            PriceUnitDescription = unit?.description ?? "",
            PriceUnitService = unit?.service ?? ""
        };
    }

    [Action("Get language CAT code", Description = "Get language CAT code")]
    public async Task<string> GetLanguageCatCodeAsync([ActionParameter] LanguageCatCodeRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.LanguageName))
            throw new PluginMisconfigurationException("Language name cannot be null or empty");

        if (string.IsNullOrWhiteSpace(input.CatType))
            throw new PluginMisconfigurationException("Category type cannot be null or empty");

        var response = await ExecuteWithRetryAcceptNull(() => AdminClient.getLanguageCatCodeAsync(Uuid, input.LanguageName, int.Parse(input.CatType)));

        if (response == null)
            throw new PluginMisconfigurationException("No language CAT code found for the given inputs.");

        return response.data;
    }

    [Action("Set item pricelist", Description = "Set a new pricelist for an item and update all related pricelines")]
    public async Task SetItemPricelist([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest item, [ActionParameter][Display("Price list ID")] string priceListID)
    {
        if (string.IsNullOrEmpty(priceListID))
        {
            throw new PluginMisconfigurationException("pricelist ID cannot be null or empty");
        }

        //Here we change set the pricelist
        await ExecuteWithRetry(() => ItemClient.setPricelistAsync(Uuid, ParseId(item.ItemId), ParseId(project.ProjectType), ParseId(priceListID)));

        //Updating all pricelines
        await ExecuteWithRetry(() => ItemClient.updatePricesAsync(Uuid, ParseId(project.ProjectType), ParseId(item.ItemId)));
    }

    [Action("Update all item prices", Description = "Updates all pricelines of an item.")]
    public async Task UpdateAllItemPrices(
        [ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest item)
    {
        await ExecuteWithRetry(() => ItemClient.updatePricesAsync(Uuid, ParseId(project.ProjectType), ParseId(item.ItemId)));
    }

    private async Task HandleLanguages(OptionalLanguageCombinationRequest languages, int projectType, int projectId,
        int itemId)
    {
        if (languages.SourceLanguageCode != null)
        {
            var sourceLanguage = await GetLanguageFromIsoOrFolderOrName(languages.SourceLanguageCode);
            var targetLanguage = await GetLanguageFromIsoOrFolderOrName(languages.TargetLanguageCode);
            var languageCombinationCode = await ExecuteWithRetry(() => ItemClient.seekLanguageCombinationAsync(Uuid, sourceLanguage.name, targetLanguage.name, projectType, projectId, itemId));

            if (languageCombinationCode == 0)
            {
                if (projectType == 3) // order
                {
                    languageCombinationCode = await ExecuteWithRetry(() => OrderClient.addLanguageCombinationAsync(Uuid, sourceLanguage.name, targetLanguage.name, itemId));
                }
                else
                {
                    languageCombinationCode = await ExecuteWithRetry(() => QuoteClient.addLanguageCombinationAsync(Uuid, sourceLanguage.name, targetLanguage.name, itemId));
                }
            }

            await ExecuteWithRetry(() => ItemClient.setLanguageCombinationIDAsync(Uuid, languageCombinationCode, projectType, itemId));
        }
    }

    [Action("Find price unit", Description = "Get price unit ID given a Service and rice unit description")]
    public async Task<ItemPriceUnitResponse> FindPriceUnit([ActionParameter] ItemPriceUnitDescriptionRequest input)
    {
        var response = await AdminClient.getAvailablePriceUnitsAsync(Uuid, Language, input.Service);

        if (response != null && response.data != null && response.data.Any())
        {
            var priceUnit = response.data.FirstOrDefault(x => x.description == input.PriceUnitDescription);

            return priceUnit == null ? new ItemPriceUnitResponse() :
                new ItemPriceUnitResponse { Id = priceUnit.priceUnitID.ToString(), Description = input.PriceUnitDescription };
        }
        return new ItemPriceUnitResponse();
    }
}