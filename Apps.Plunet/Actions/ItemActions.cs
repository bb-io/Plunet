using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Item;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Plugins.Plunet.DataItem30Service;

namespace Apps.Plunet.Actions
{
    [ActionList]
    public class ItemActions : PlunetInvocable
    {
        private readonly IFileManagementClient _fileManagementClient;
        public ItemActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(invocationContext)
        {
            _fileManagementClient = fileManagementClient;
        }

        [Action("Search items",
            Description = "Search for items either under a specific order/quote or with a certain status")]
        public async Task<ListItemResponse> SearchItems([ActionParameter] OptionalItemProjectRequest item,
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
                    result = await ItemClient.getItemsByStatus1Async(Uuid, ParseId(item.ProjectType),
                        ParseId(searchParams.Status));
                }
                else
                {
                    result = currencyParams.CurrencyType == null
                        ? await ItemClient.getItemsByStatus3Async(Uuid, ParseId(item.ProjectType),
                            ParseId(searchParams.Status), ParseId(searchParams.DocumentStatus))
                        : await ItemClient.getItemsByStatus3ByCurrencyTypeAsync(Uuid, ParseId(item.ProjectType),
                            ParseId(searchParams.Status), ParseId(searchParams.DocumentStatus),
                            ParseId(currencyParams.CurrencyType));
                }
            }
            else
            {
                if (searchParams.Status == null)
                {
                    result = currencyParams.CurrencyType == null
                        ? await ItemClient.getAllItemObjectsAsync(Uuid, ParseId(item.ProjectId),
                            ParseId(item.ProjectType))
                        : await ItemClient.getAllItemObjectsByCurrencyAsync(Uuid, ParseId(item.ProjectId),
                            ParseId(item.ProjectType), ParseId(currencyParams.CurrencyType));
                }
                else if (searchParams.DocumentStatus == null)
                {
                    result = await ItemClient.getItemsByStatus2Async(Uuid, ParseId(item.ProjectId),
                        ParseId(item.ProjectType), ParseId(searchParams.Status));
                }
                else
                {
                    result = currencyParams.CurrencyType == null
                        ? await ItemClient.getItemsByStatus4Async(Uuid, ParseId(item.ProjectId),
                            ParseId(item.ProjectType), ParseId(searchParams.Status),
                            ParseId(searchParams.DocumentStatus))
                        : await ItemClient.getItemsByStatus4ByCurrencyTypeAsync(Uuid, ParseId(item.ProjectId),
                            ParseId(item.ProjectType), ParseId(searchParams.Status),
                            ParseId(searchParams.DocumentStatus), ParseId(currencyParams.CurrencyType));
                }
            }

            if (result.statusMessage != ApiResponses.Ok)
                throw new(result.statusMessage);

            return new ListItemResponse
            {
                Items = result.data is null ? new List<ItemResponse>() : result.data.Select(x => new ItemResponse(x))
            };
        }


        [Action("Get item", Description = "Get details for a Plunet item")]
        public async Task<ItemResponse> GetItem([ActionParameter] ProjectTypeRequest project,
            [ActionParameter] GetItemRequest request, [ActionParameter] OptionalCurrencyTypeRequest currency)
        {
            var result = currency.CurrencyType == null
                ? await ItemClient.getItemObjectAsync(Uuid, ParseId(project.ProjectType), ParseId(request.ItemId))
                : await ItemClient.getItemObjectByCurrencyTypeAsync(Uuid, ParseId(project.ProjectType),
                    ParseId(request.ItemId), ParseId(currency.CurrencyType));

            if (result.statusMessage != ApiResponses.Ok)
                throw new(result.statusMessage);

            return new(result.data);
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
                ? await ItemClient.insertLanguageIndependentItemAsync(Uuid, itemIn)
                : await ItemClient.insert2Async(Uuid, itemIn);

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
            var response = await ItemClient.deleteAsync(Uuid, ParseId(request.ItemId), ParseId(project.ProjectType));

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

            var response = await ItemClient.updateAsync(Uuid, itemIn, false);

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            var itemRes = await ItemClient.getItemObjectAsync(Uuid, ParseId(project.ProjectType), ParseId(item.ItemId));
            await HandleLanguages(languages, ParseId(project.ProjectType), itemRes.data.projectID,
                ParseId(item.ItemId));

            return await GetItem(project, item, new OptionalCurrencyTypeRequest { });
        }

        [Action("Get item pricelines", Description = "Get a list of all pricelines attached to an item")]
        public async Task<PricelinesResponse> GetItemPricelines([ActionParameter] ProjectTypeRequest project,
            [ActionParameter] GetItemRequest item)
        {
            var response =
                await ItemClient.getPriceLine_ListAsync(Uuid, ParseId(item.ItemId), ParseId(project.ProjectType));

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            return new PricelinesResponse
            {
                Pricelines = response.data.Select(CreatePricelineResponse),
            };
        }

        [Action("Create item priceline", Description = "Add a new pricline to an item")]
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

            var response = await ItemClient.insertPriceLineAsync(Uuid, ParseId(item.ItemId),
                ParseId(project.ProjectType), pricelineIn, false);

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            return CreatePricelineResponse(response.data);
        }

        [Action("Delete item priceline", Description = "Delete a priceline from an item")]
        public async Task DeletePriceline([ActionParameter] ProjectTypeRequest project,
            [ActionParameter] GetItemRequest item, [ActionParameter] PricelineIdRequest line)
        {
            var response = await ItemClient.deletePriceLineAsync(Uuid, ParseId(item.ItemId),
                ParseId(project.ProjectType), ParseId(line.Id));

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);
        }

        [Action("Update item priceline", Description = "Update an existing item pricline")]
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

            var response = await ItemClient.updatePriceLineAsync(Uuid, ParseId(item.ItemId),
                ParseId(project.ProjectType), pricelineIn);

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            return CreatePricelineResponse(response.data);
        }

        [Action("Upload CAT report",
            Description = "Upload a report file into the report folder of the specified item.")]
        public async Task UploadCatReport(
            [ActionParameter] GetItemRequest item,
            [ActionParameter] UploadCatReportRequest input)
        {
            var fileBytes = _fileManagementClient.DownloadAsync(input.File).Result.GetByteData().Result;
            var response = await ItemClient.setCatReport2Async(Uuid, fileBytes, input.File.Name, fileBytes.Length,
                input.OverwriteExistingPricelines, ParseId(input.CatType), ParseId(input.ProjectType),
                input.CopyResultsToItem, ParseId(item.ItemId));
            
            if (response.Result.statusMessage != ApiResponses.Ok)
                throw new(response.Result.statusMessage);
        }

        private PricelineResponse CreatePricelineResponse(PriceLine line)
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

        private async Task HandleLanguages(OptionalLanguageCombinationRequest languages, int projectType, int projectId,
            int itemId)
        {
            if (languages.SourceLanguageCode != null)
            {
                var sourceLanguage = await GetLanguageFromIsoOrFolderOrName(languages.SourceLanguageCode);
                var targetLanguage = await GetLanguageFromIsoOrFolderOrName(languages.TargetLanguageCode);
                var languageCombination = await ItemClient.seekLanguageCombinationAsync(Uuid, sourceLanguage.name,
                    targetLanguage.name, projectType, projectId, itemId);
                var languageCombinationCode = languageCombination.data;

                if (languageCombinationCode == 0)
                {
                    if (projectType == 3) // order
                    {
                        var result = await OrderClient.addLanguageCombinationAsync(Uuid, sourceLanguage.name,
                            targetLanguage.name, itemId);
                        languageCombinationCode = result.data;
                    }
                    else
                    {
                        var result = await QuoteClient.addLanguageCombinationAsync(Uuid, sourceLanguage.name,
                            targetLanguage.name, itemId);
                        languageCombinationCode = result.data;
                    }
                }

                await ItemClient.setLanguageCombinationIDAsync(Uuid, languageCombinationCode, projectType, itemId);
            }
        }

        // Pricelist
        // Copy jobs from workflow
        // SetCatReport
    }
}