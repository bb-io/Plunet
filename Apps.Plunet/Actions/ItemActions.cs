using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Item;
using Apps.Plunet.Models.Request.Request;
using Apps.Plunet.Models.Request.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataItem30Service;
using System;

namespace Apps.Plunet.Actions;

[ActionList]
public class ItemActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{
    [Action("Search items",
        Description = "Search for items either under a specific order/quote or with a certain status")]
    public async Task<SearchResponse<ItemResponse>> SearchItems([ActionParameter] OptionalItemProjectRequest item,
        [ActionParameter] SearchItemsRequest searchParams,
        [ActionParameter] OptionalCurrencyTypeRequest currencyParams)
    {
        ItemListResult result;

        if (item.ProjectId == null)
        {
            if (searchParams.Status == null)
            {
                throw new Exception("Please provide either an order or quote ID or an item status.");
            }

            if (searchParams.DocumentStatus == null)
            {
                result = await ExecuteWithRetry<ItemListResult>(async () => await ItemClient.getItemsByStatus1Async(Uuid, ParseId(item.ProjectType),
                    ParseId(searchParams.Status)));
            }
            else
            {
                result = currencyParams.CurrencyType == null
                    ? await ExecuteWithRetry<ItemListResult>(async () => await ItemClient.getItemsByStatus3Async(Uuid, ParseId(item.ProjectType),
                        ParseId(searchParams.Status), ParseId(searchParams.DocumentStatus)))
                    : await ExecuteWithRetry<ItemListResult>(async () => await ItemClient.getItemsByStatus3ByCurrencyTypeAsync(Uuid, ParseId(item.ProjectType),
                        ParseId(searchParams.Status), ParseId(searchParams.DocumentStatus),
                        ParseId(currencyParams.CurrencyType)));
            }
        }
        else
        {
            if (searchParams.Status == null)
            {
                result = currencyParams.CurrencyType == null
                    ? await ExecuteWithRetry<ItemListResult>(async () => await ItemClient.getAllItemObjectsAsync(Uuid, ParseId(item.ProjectId),
                        ParseId(item.ProjectType)))
                    : await ExecuteWithRetry<ItemListResult>(async () => await ItemClient.getAllItemObjectsByCurrencyAsync(Uuid, ParseId(item.ProjectId),
                        ParseId(item.ProjectType), ParseId(currencyParams.CurrencyType)));
            }
            else if (searchParams.DocumentStatus == null)
            {
                result = await ExecuteWithRetry<ItemListResult>(async () => await ItemClient.getItemsByStatus2Async(Uuid, ParseId(item.ProjectId),
                    ParseId(item.ProjectType), ParseId(searchParams.Status)));
            }
            else
            {
                result = currencyParams.CurrencyType == null
                    ? await ExecuteWithRetry<ItemListResult>(async () => await ItemClient.getItemsByStatus4Async(Uuid, ParseId(item.ProjectId),
                        ParseId(item.ProjectType), ParseId(searchParams.Status),
                        ParseId(searchParams.DocumentStatus)))
                    : await ExecuteWithRetry<ItemListResult>(async () => await ItemClient.getItemsByStatus4ByCurrencyTypeAsync(Uuid, ParseId(item.ProjectId),
                        ParseId(item.ProjectType), ParseId(searchParams.Status),
                        ParseId(searchParams.DocumentStatus), ParseId(currencyParams.CurrencyType)));
            }
        }

        if (result.statusMessage != ApiResponses.Ok)
            throw new(result.statusMessage);

        var projectType = (ItemProjectType)int.Parse(item.ProjectType);
        var items = result.data is null
            ? new List<ItemResponse>()
            : result.data.Take(searchParams.Limit ?? SystemConsts.SearchLimit).Select(x => new ItemResponse(x, projectType)).ToList();
        return new SearchResponse<ItemResponse>(items);
    }
    
    [Action("Find item", Description = "Find a specific item based on specific criteria")]
    public async Task<FindResponse<ItemResponse>> FindItem([ActionParameter] OptionalItemProjectRequest item,
        [ActionParameter] SearchItemsRequest searchParams,
        [ActionParameter] OptionalCurrencyTypeRequest currencyParams)
    {
        var result = await SearchItems(item, searchParams, currencyParams);
        return new(result.Items.FirstOrDefault(), result.TotalCount);
    }

    [Action("Get item", Description = "Get details for a Plunet item")]
    public async Task<ItemResponse> GetItem([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest request, [ActionParameter] OptionalCurrencyTypeRequest currency)
    {
        var result = currency.CurrencyType == null
            ? await ExecuteWithRetry<ItemResult>(async () => await ItemClient.getItemObjectAsync(Uuid, ParseId(project.ProjectType), ParseId(request.ItemId)))
            : await ExecuteWithRetry<ItemResult>(async () => await ItemClient.getItemObjectByCurrencyTypeAsync(Uuid, ParseId(project.ProjectType),
                ParseId(request.ItemId), ParseId(currency.CurrencyType)));

        if (result.statusMessage != ApiResponses.Ok)
            throw new(result.statusMessage);

        var projectType = (ItemProjectType)int.Parse(project.ProjectType);
        return new(result.data, projectType);
    }

    [Action("Create item", Description = "Create a new item in Plunet")]
    public async Task<ItemResponse> CreateItem([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] ProjectIdRequest projectId, [ActionParameter] CreateItemRequest request,
        [ActionParameter] OptionalLanguageCombinationRequest languages)
    {
        if ((languages.SourceLanguageCode == null) != (languages.TargetLanguageCode == null))
        {
            throw new Exception("Either both source and target languages should be defined or neither");
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
        };

        var response = languages.SourceLanguageCode == null
            ? await ExecuteWithRetry<IntegerResult>(async () => await ItemClient.insertLanguageIndependentItemAsync(Uuid, itemIn))
            : await ExecuteWithRetry<IntegerResult>(async () => await ItemClient.insert2Async(Uuid, itemIn));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        await HandleLanguages(languages, ParseId(project.ProjectType), ParseId(projectId.ProjectId), response.data);

        return await GetItem(project, new GetItemRequest { ItemId = response.data.ToString() },
            new OptionalCurrencyTypeRequest { });
    }

    [Action("Delete item", Description = "Delete a Plunet item")]
    public async Task DeleteItem([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest request)
    {
        var response = await ExecuteWithRetry<ItemListResult>(async () => await ItemClient.deleteAsync(Uuid, ParseId(request.ItemId), ParseId(project.ProjectType)));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);
    }

    [Action("Update item", Description = "Update an existing item in Plunet")]
    public async Task<ItemResponse> UpdateItem([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest item, [ActionParameter] CreateItemRequest request,
        [ActionParameter] OptionalLanguageCombinationRequest languages)
    {
        if ((languages.SourceLanguageCode == null) != (languages.TargetLanguageCode == null))
        {
            throw new Exception("Either both source and target languages should be defined or neither");
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

        var response = await ExecuteWithRetry<Result>(async () => await ItemClient.updateAsync(Uuid, itemIn, false));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        var itemRes = await ExecuteWithRetry<ItemResult>(async () => await ItemClient.getItemObjectAsync(Uuid, ParseId(project.ProjectType), ParseId(item.ItemId)));
        await HandleLanguages(languages, ParseId(project.ProjectType), itemRes.data.projectID,
            ParseId(item.ItemId));

        return await GetItem(project, item, new OptionalCurrencyTypeRequest { });
    }

    [Action("Get item pricelines", Description = "Get a list of all pricelines attached to an item")]
    public async Task<PricelinesResponse> GetItemPricelines([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest item)
    {
        var response =
            await ExecuteWithRetry<PriceLineListResult>(async () => await ItemClient.getPriceLine_ListAsync(Uuid, ParseId(item.ItemId), ParseId(project.ProjectType)));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        if (response.data is null)
        {
            return new PricelinesResponse();
        }

        var result = new List<PricelineResponse>();

        foreach (var priceLine in response.data) 
        {
            var priceUnit = await ItemClient.getPriceUnitAsync(Uuid, priceLine.priceUnitID, Language);
            result.Add(CreatePricelineResponse(priceLine,priceUnit.data));
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

        var response = await ExecuteWithRetry<PriceLineResult>(async () => await ItemClient.insertPriceLineAsync(Uuid, ParseId(item.ItemId),
            ParseId(project.ProjectType), pricelineIn, false));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        var priceUnit = await ItemClient.getPriceUnitAsync(Uuid, int.Parse(unit.PriceUnit) , Language);
        return CreatePricelineResponse(response.data, priceUnit.data);
    }

    [Action("Delete item priceline", Description = "Delete a priceline from an item")]
    public async Task DeletePriceline([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest item, [ActionParameter] PricelineIdRequest line)
    {
        var response = await ExecuteWithRetry<Result>(async () => await ItemClient.deletePriceLineAsync(Uuid, ParseId(item.ItemId),
            ParseId(project.ProjectType), ParseId(line.Id)));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);
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

        var response = await ExecuteWithRetry<PriceLineResult>(async () => await ItemClient.updatePriceLineAsync(Uuid, ParseId(item.ItemId),
            ParseId(project.ProjectType), pricelineIn));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);
       

        var priceUnit = await ItemClient.getPriceUnitAsync(Uuid, int.Parse(unit.PriceUnit), Language);
        return CreatePricelineResponse(response.data, priceUnit.data);
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

    [Action("Get language CAT code", Description = "Get language CAT code")]
    public async Task<LanguageCatCodeResponse> GetLanguageCatCodeAsync([ActionParameter]  LanguageCatCodeRequest input)
    {
        if (string.IsNullOrWhiteSpace(Uuid))
            throw new ArgumentException("UUID cannot be null or empty", nameof(Uuid));

        if (string.IsNullOrWhiteSpace(input.LanguageName))
            throw new ArgumentException("Language name cannot be null or empty", nameof(input.LanguageName));

        if (string.IsNullOrWhiteSpace(input.CatType))
            throw new ArgumentException("Category type cannot be null or empty", nameof(input.CatType));

        try 
        {
            var response = await AdminClient.getLanguageCatCodeAsync(Uuid, input.LanguageName,input.CatType);

            if (response == null || string.IsNullOrEmpty(response.ToString()))
                throw new Exception("No language CAT code found for the given inputs.");

            return new LanguageCatCodeResponse
            {
                Text = response.ToString()
            };
        }
        catch (Exception ex)
        {
            throw new (ex.Message);
        }
    }

    [Action("Set item pricelist", Description ="Set a new pricelist for an item and update all related pricelines")]
    public async Task<Result> SetItemPricelist([ActionParameter] ProjectTypeRequest project,
        [ActionParameter] GetItemRequest item, [ActionParameter][Display("Price list ID")]string priceListID)
    {
        if (string.IsNullOrEmpty(priceListID))
        {
            throw new ArgumentNullException("pricelist ID cannot be null or empty",nameof(priceListID));
        }

        //Here we change set the pricelist
        var setPriceListResult = await ExecuteWithRetry<Result>(async() =>
        await ItemClient.setPricelistAsync(Uuid,ParseId(item.ItemId), ParseId(project.ProjectType), ParseId(priceListID)));

        if (setPriceListResult.statusMessage != ApiResponses.Ok)
        {
            throw new(setPriceListResult.statusMessage);
        }

        //Updating all pricelines
        var updatePriceResult = await ExecuteWithRetry<Result>(async() => 
        await ItemClient.updatePricesAsync(Uuid, ParseId(project.ProjectType), ParseId(item.ItemId)));

        if (updatePriceResult.statusMessage != ApiResponses.Ok)
        {
            throw new(updatePriceResult.statusMessage );
        }

        return new Result
        {
            statusMessage = ApiResponses.Ok
        };
    }

    private async Task HandleLanguages(OptionalLanguageCombinationRequest languages, int projectType, int projectId,
        int itemId)
    {
        if (languages.SourceLanguageCode != null)
        {
            var sourceLanguage = await GetLanguageFromIsoOrFolderOrName(languages.SourceLanguageCode);
            var targetLanguage = await GetLanguageFromIsoOrFolderOrName(languages.TargetLanguageCode);
            var languageCombination = await ExecuteWithRetry<IntegerResult>(async () => await ItemClient.seekLanguageCombinationAsync(Uuid, sourceLanguage.name,
                targetLanguage.name, projectType, projectId, itemId));
            var languageCombinationCode = languageCombination.data;

            if (languageCombinationCode == 0)
            {
                if (projectType == 3) // order
                {
                    var result = await ExecuteWithRetry<Blackbird.Plugins.Plunet.DataOrder30Service.IntegerResult>(async () => await OrderClient.addLanguageCombinationAsync(Uuid, sourceLanguage.name,
                        targetLanguage.name, itemId));
                    languageCombinationCode = result.data;
                }
                else
                {
                    var result = await ExecuteWithRetry<Blackbird.Plugins.Plunet.DataQuote30Service.IntegerResult>(async () => await QuoteClient.addLanguageCombinationAsync(Uuid, sourceLanguage.name,
                        targetLanguage.name, itemId));
                    languageCombinationCode = result.data;
                }
            }

            await ExecuteWithRetry<Result>(async () => await ItemClient.setLanguageCombinationIDAsync(Uuid, languageCombinationCode, projectType, itemId));
        }
    }

    // Pricelist
    // Copy jobs from workflow
    // SetCatReport
    
    private async Task<T> ExecuteWithRetry<T>(Func<Task<Result>> func, int maxRetries = 10, int delay = 1000)
        where T : Result
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();

            if (result.statusMessage == ApiResponses.Ok)
            {
                return (T)result;
            }

            if (result.statusMessage.Contains("session-UUID used is invalid") && attempts < maxRetries)
            {
                await Task.Delay(delay);
                await RefreshAuthToken();
                attempts++;
                continue;
            }

            return (T)result;
        }
    }

    private async Task<T> ExecuteWithRetry<T>(Func<Task<Blackbird.Plugins.Plunet.DataOrder30Service.Result>> func,
        int maxRetries = 10, int delay = 1000)
        where T : Blackbird.Plugins.Plunet.DataOrder30Service.Result
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();

            if (result.statusMessage == ApiResponses.Ok)
            {
                return (T)result;
            }

            if (result.statusMessage.Contains("session-UUID used is invalid") && attempts < maxRetries)
            {
                await Task.Delay(delay);
                await RefreshAuthToken();
                attempts++;
                continue;
            }

            return (T)result;
        }
    }
    
    private async Task<T> ExecuteWithRetry<T>(Func<Task<Blackbird.Plugins.Plunet.DataQuote30Service.Result>> func,
        int maxRetries = 10, int delay = 1000)
        where T : Blackbird.Plugins.Plunet.DataQuote30Service.Result
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();

            if (result.statusMessage == ApiResponses.Ok)
            {
                return (T)result;
            }

            if(result.statusMessage.Contains("session-UUID used is invalid"))
            {
                if (attempts < maxRetries)
                {
                    await Task.Delay(delay);
                    await RefreshAuthToken();
                    attempts++;
                    continue;
                }

                throw new($"No more retries left. Last error: {result.statusMessage}, Session UUID used is invalid.");
            }

            return (T)result;
        }
    }
}