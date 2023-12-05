using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Item;
using Apps.Plunet.Models.Order;
using Apps.Plunet.Models.Request.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
using Blackbird.Plugins.Plunet.DataAdmin30Service;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using Microsoft.Win32;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Actions
{
    [ActionList]
    public class ItemActions : PlunetInvocable
    {
        public ItemActions(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        [Action("Search items", Description = "Search for items either under a specific order/quote or with a certain status")]
        public async Task<ListItemResponse> SearchItems([ActionParameter] OptionalItemProjectRequest item, [ActionParameter] SearchItemsRequest searchParams, [ActionParameter] OptionalCurrencyTypeRequest currencyParams)
        {
            ItemListResult result;

            if (searchParams.Status == null)
            {
                result = currencyParams.CurrencyType == null ? await ItemClient.getAllItemObjectsAsync(Uuid, ParseId(item.ProjectId), ParseId(item.ProjectType)) :
                    await ItemClient.getAllItemObjectsByCurrencyAsync(Uuid, ParseId(item.ProjectId), ParseId(item.ProjectType), ParseId(currencyParams.CurrencyType));
            }
            else
            {
                if (item.ProjectId == null)
                {
                    if (searchParams.DocumentStatus == null)
                    {
                        result = await ItemClient.getItemsByStatus1Async(Uuid, ParseId(item.ProjectType), ParseId(searchParams.Status));
                    }
                    else
                    {
                        result = currencyParams.CurrencyType == null ? await ItemClient.getItemsByStatus3Async(Uuid, ParseId(item.ProjectType), ParseId(searchParams.Status), ParseId(searchParams.DocumentStatus)) :
                            await ItemClient.getItemsByStatus3ByCurrencyTypeAsync(Uuid, ParseId(item.ProjectType), ParseId(searchParams.Status), ParseId(searchParams.DocumentStatus), ParseId(currencyParams.CurrencyType));
                    }                    
                }
                else
                {
                    if (searchParams.DocumentStatus == null)
                    {
                        result = await ItemClient.getItemsByStatus2Async(Uuid, ParseId(item.ProjectId), ParseId(item.ProjectType), ParseId(searchParams.Status));
                    }
                    else
                    {
                        result = currencyParams.CurrencyType == null ? await ItemClient.getItemsByStatus4Async(Uuid, ParseId(item.ProjectId), ParseId(item.ProjectType), ParseId(searchParams.Status), ParseId(searchParams.DocumentStatus)) :
                            await ItemClient.getItemsByStatus4ByCurrencyTypeAsync(Uuid, ParseId(item.ProjectId), ParseId(item.ProjectType), ParseId(searchParams.Status), ParseId(searchParams.DocumentStatus), ParseId(currencyParams.CurrencyType));
                    }
                }
            }

            if (result.statusMessage != ApiResponses.Ok)
                throw new(result.statusMessage);

            return new ListItemResponse
            {
                Items = result.data.Select(x => new ItemResponse(x))
            };
        }


        [Action("Get item", Description = "Get details for a Plunet item")]
        public async Task<ItemResponse> GetItem([ActionParameter] ProjectTypeRequest project, [ActionParameter] GetItemRequest request, [ActionParameter] OptionalCurrencyTypeRequest currency)
        {
            var result = currency.CurrencyType == null ? await ItemClient.getItemObjectAsync(Uuid, ParseId(project.ProjectType), ParseId(request.ItemId)) :
                await ItemClient.getItemObjectByCurrencyTypeAsync(Uuid, ParseId(project.ProjectType), ParseId(request.ItemId), ParseId(currency.CurrencyType));

            if (result.statusMessage != ApiResponses.Ok)
                throw new(result.statusMessage);

            return new(result.data);
        }

        [Action("Create item", Description = "Create a new item in Plunet")]
        public async Task<ItemResponse> CreateItem([ActionParameter] ProjectTypeRequest project, [ActionParameter] ProjectIdRequest projectId, [ActionParameter] CreateItemRequest request, [ActionParameter] OptionalLanguageCombinationRequest languages)
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

            var response = languages.SourceLanguageCode == null ? await ItemClient.insertLanguageIndependentItemAsync(Uuid, itemIn) : await ItemClient.insert2Async(Uuid, itemIn);

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            await HandleLanguages(languages, ParseId(project.ProjectType), ParseId(projectId.ProjectId), response.data);

            return await GetItem(project, new GetItemRequest { ItemId = response.data.ToString() }, new OptionalCurrencyTypeRequest { });
        }

        [Action("Delete item", Description = "Delete a Plunet item")]
        public async Task DeleteItem([ActionParameter] ProjectTypeRequest project, [ActionParameter] GetItemRequest request)
        {
            await ItemClient.deleteAsync(Uuid, ParseId(request.ItemId), ParseId(project.ProjectType));
        }

        [Action("Update item", Description = "Update an existing item in Plunet")]
        public async Task<ItemResponse> UpdateItem([ActionParameter] ProjectTypeRequest project, [ActionParameter] GetItemRequest item, [ActionParameter] CreateItemRequest request, [ActionParameter] OptionalLanguageCombinationRequest languages)
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
            await HandleLanguages(languages, ParseId(project.ProjectType), itemRes.data.projectID, ParseId(item.ItemId));            

            return await GetItem(project, item, new OptionalCurrencyTypeRequest { });
        }

        private async Task HandleLanguages(OptionalLanguageCombinationRequest languages, int projectType, int projectId, int itemId)
        {
            if (languages.SourceLanguageCode != null)
            {
                var sourceLanguage = await GetLanguageFromIsoOrFolderOrName(languages.SourceLanguageCode);
                var targetLanguage = await GetLanguageFromIsoOrFolderOrName(languages.TargetLanguageCode);
                var languageCombination = await ItemClient.seekLanguageCombinationAsync(Uuid, sourceLanguage.name, targetLanguage.name, projectType, projectId, itemId);
                var languageCombinationCode = languageCombination.data;

                if (languageCombinationCode == 0)
                {
                    if (projectType == 3) // order
                    {
                        var result = await OrderClient.addLanguageCombinationAsync(Uuid, sourceLanguage.name, targetLanguage.name, itemId);
                        languageCombinationCode = result.data;
                    }
                    else
                    {
                        var result = await QuoteClient.addLanguageCombinationAsync(Uuid, sourceLanguage.name, targetLanguage.name, itemId);
                        languageCombinationCode = result.data;
                    }
                }

                await ItemClient.setLanguageCombinationIDAsync(Uuid, languageCombinationCode, projectType, itemId);
            }
        }

        // Pricelist
        // Copy jobs from workflow
        // GetJobs? GetJobsWithStatus?
        // SetCatReport

        // Add pricelines (from priceline transfer file)
        // Get pricelines (to priceline transfer file), optional currency
        // Update priceline
    }
}
