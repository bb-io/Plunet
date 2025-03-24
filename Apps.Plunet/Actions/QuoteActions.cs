using Apps.Plunet.Constants;
using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Order;
using Apps.Plunet.Models.Quote.Request;
using Apps.Plunet.Models.Quote.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataQuote30Service;
using IntegerArrayResult = Blackbird.Plugins.Plunet.DataQuote30Service.IntegerArrayResult;
using IntegerResult = Blackbird.Plugins.Plunet.DataQuote30Service.IntegerResult;
using Result = Blackbird.Plugins.Plunet.DataQuote30Service.Result;

namespace Apps.Plunet.Actions;

[ActionList]
public class QuoteActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{
    [Action("Search quotes", Description = "Search for specific quotes based on specific criteria")]
    public async Task<SearchResponse<QuoteResponse>> SearchQuotes([ActionParameter] SearchQuotesInput input)
    {
        var searchResult = await ExecuteWithRetryAcceptNull(() => QuoteClient.searchAsync(Uuid,
            new()
            {
                sourceLanguage = input.SourceLanguage ?? string.Empty,
                targetLanguage = input.TargetLanguage ?? string.Empty,
                quoteStatus = ParseId(input.QuoteStatus),
                timeFrame = input.DateFrom is not null || input.DateTo is not null
                    ? new()
                    {
                        dateFrom = input.DateFrom ?? default,
                        dateTo = input.DateTo ?? default
                    }
                    : default,
                selectionEntryCustomer = input.CustomerMainId is not null || input.CustomerEntryType is not null
                    ? new()
                    {
                        mainID = ParseId(input.CustomerMainId),
                        customerEntryType = ParseId(input.CustomerEntryType)
                    }
                    : default,
                SelectionEntry_Resource = input.ResourceMainId is not null || input.ResourceEntryType is not null
                    ? new()
                    {
                        mainID = ParseId(input.ResourceMainId),
                        resourceEntryType = ParseId(input.ResourceEntryType)
                    }
                    : default
            }));

        if (searchResult is null)
            return new();

        var results = new List<QuoteResponse>();
        foreach (var id in searchResult.Where(x => x.HasValue).Take(input.Limit ?? SystemConsts.SearchLimit))
        {
            var quoteResponse = await GetQuote(new GetQuoteRequest { QuoteId = id.Value.ToString() });
            results.Add(quoteResponse);
        }

        return new(results);
    }

    [Action("Find quote", Description = "Find a quote based on specific criteria")]
    public async Task<FindResponse<QuoteResponse>> FindQuote([ActionParameter] SearchQuotesInput request)
    {
        var searchResult = await SearchQuotes(request);
        return new(searchResult.Items.FirstOrDefault(), searchResult.TotalCount);
    }

    [Action("Get quote", Description = "Get details for a Plunet quote")]
    public async Task<QuoteResponse> GetQuote([ActionParameter] GetQuoteRequest request)
    {
        var quote = await ExecuteWithRetry(() => QuoteClient.getQuoteObjectAsync(Uuid, ParseId(request.QuoteId)));

        var items = await ExecuteWithRetryAcceptNull(() => ItemClient.getAllItemObjectsAsync(Uuid, ParseId(request.QuoteId), 1));

        var totalPrice = items?.Sum(x => x.totalPrice) ?? 0;

        var customerId = await ExecuteWithRetryAcceptNull(() => QuoteClient.getCustomerIDAsync(Uuid, ParseId(request.QuoteId)));

        var contactId = await ExecuteWithRetryAcceptNull(() => QuoteClient.getCustomerContactIDAsync(Uuid, ParseId(request.QuoteId)));

        var pmId = await ExecuteWithRetryAcceptNull(() => QuoteClient.getProjectmanagerIDAsync(Uuid, ParseId(request.QuoteId)));

        var orderId = await ExecuteWithRetryAcceptNull(() => QuoteClient.getOrderIDFirstItemAsync(Uuid, ParseId(request.QuoteId)));

        var category = await ExecuteWithRetryAcceptNull(() => QuoteClient.getProjectCategoryAsync(Uuid, Language, ParseId(request.QuoteId)));

        var projectStatus = await ExecuteWithRetry(() => QuoteClient.getProjectStatusAsync(Uuid, ParseId(request.QuoteId)));

        var sourceLanguages = items?.Where(x => x.sourceLanguage != null).Select(x => x.sourceLanguage).Distinct().ToList() ?? new();
        var targetLanguages = items?.Where(x => x.targetLanguage != null).Select(x => x.targetLanguage).Distinct().ToList() ?? new();
        var languageCombinationsStrings = items?.Where(x => x.sourceLanguage != null && x.targetLanguage != null).Select(x => $"{x.sourceLanguage} - {x.targetLanguage}").Distinct().ToList() ?? new();
        
        var sourceLanguageCodes = await GetLanguageCodes(sourceLanguages);
        var targetLanguageCodes = await GetLanguageCodes(targetLanguages);
        var languageCombinations = await ParseLanguageCombinations(languageCombinationsStrings);

        return new(quote)
        {
            TotalPrice = totalPrice,
            CustomerId = customerId?.ToString(),
            ContactId = contactId?.ToString(),
            ProjectManagerId = pmId?.ToString() ?? string.Empty,
            OrderId = orderId?.ToString(),
            ProjectCategory = category ?? string.Empty,
            ProjectStatus = projectStatus.ToString(),
            ItemsSourceLanguages = sourceLanguageCodes,
            ItemsTargetLanguages = targetLanguageCodes,
            LanguageCombinations = languageCombinations
        };
    }
    
    [Action("Get quote item target languages for source",
        Description = "Given a source language and an quote item")]
    public async Task<LanguagesResponse> GetQuoteTargetLanguage([ActionParameter] GetQuoteRequest request,
        [ActionParameter] SourceLanguageRequest language)
    {
        var quote = await GetQuote(request);
        var response = quote.LanguageCombinations.Where(x => x.Source == language.SourceLanguageCode)
            .Select(x => x.Target).Distinct();

        return new LanguagesResponse
        {
            Languages = response,
        };
    }

    [Action("Create quote", Description = "Create a new quote in Plunet, optionally using a template")]
    public async Task<QuoteResponse> CreateQuote([ActionParameter] CreateQuoteRequest request,
        QuoteTemplateRequest template)
    {
        var quoteIn = new QuoteIN
        {
            projectName = request.ProjectName,
            customerID = ParseId(request.CustomerId),
            subject = request.Subject,
            creationDate = DateTime.Now,
            currency = request.Currency,
            projectManagerMemo = request.ProjectManagerMemo,
            referenceNumber = request.ReferenceNumber,
            status = ParseId(request.Status)
        };

        var quoteId = template.TemplateId == null
            ? await ExecuteWithRetry(() => QuoteClient.insert2Async(Uuid, quoteIn))
            : await ExecuteWithRetry(() => QuoteClient.insert_byTemplateAsync(Uuid, quoteIn, ParseId(template.TemplateId)));

        if (request.RequestId is not null)
            await ExecuteWithRetry(() => QuoteClient.setRequestIDAsync(Uuid, quoteId, ParseId(request.RequestId)));

        if (request.ProjectStatus is not null)
            await ExecuteWithRetry(() => QuoteClient.setProjectStatusAsync(Uuid, quoteId, ParseId(request.ProjectStatus)));

        if (request.ProjectManagerId is not null)
            await ExecuteWithRetry(() => QuoteClient.setProjectmanagerIDAsync(Uuid, ParseId(request.ProjectManagerId), quoteId));

        if (request.ProjectCategory is not null)
            await ExecuteWithRetry(() => QuoteClient.setProjectCategoryAsync(Uuid, request.ProjectCategory, Language, quoteId));

        if (request.ExternalId is not null)
            await ExecuteWithRetry(() => QuoteClient.setExternalIDAsync(Uuid, quoteId, request.ExternalId));

        if (request.ContactId is not null)
            await ExecuteWithRetry(() => QuoteClient.setCustomerContactIDAsync(Uuid, ParseId(request.ContactId), quoteId));

        return await GetQuote(new GetQuoteRequest { QuoteId = quoteId.ToString() });
    }

    [Action("Create quote from template", Description = "Create a new quote in Plunet using a template")]
    public async Task<QuoteResponse> CreateQuoteFromTemplate([ActionParameter] CreateQuoteRequest request,
        [ActionParameter, Display("Template"), DataSource(typeof(QuoteTemplateDataHandler))] string templateId)
    {
        var quoteTemplateRequest = new QuoteTemplateRequest { TemplateId = templateId };
        return await CreateQuote(request, quoteTemplateRequest);
    }

    //[Action("Add language combination to quote", Description = "Add a new language combination to an existing quote")]
    //public async Task<AddLanguageCombinationResponse> AddLanguageCombinationToQuote(IEnumerable<AuthenticationCredentialsProvider> Creds, [ActionParameter] AddLanguageCombinationToQuoteRequest request)
    //{
    //    var uuid = Creds.GetAuthToken();
    //    using var quoteClient = Clients.GetQuoteClient(Creds.GetInstanceUrl());
    //    var langCombination =
    //        GetLanguageNamesCombinationByLanguageCodeIso(uuid, request.SourceLanguageCode,
    //            request.TargetLanguageCode);
    //    if (string.IsNullOrEmpty(langCombination.TargetLanguageName))
    //    {
    //        await Creds.Logout();
    //        return new AddLanguageCombinationResponse();
    //    }

    //    var result = await quoteClient.addLanguageCombinationAsync(uuid, langCombination.SourceLanguageName,
    //        langCombination.TargetLanguageName, request.QuoteId);
    //    await Creds.Logout();
    //    return new AddLanguageCombinationResponse { LanguageCombinationId = result.data };
    //}

    //[Action("Request order to quote", Description = "Request order to a Plunet quote")]
    //public async Task RequestOrder(IEnumerable<AuthenticationCredentialsProvider> Creds, [ActionParameter] int quoteId)
    //{
    //    var uuid = Creds.GetAuthToken();
    //    using var quoteClient = Clients.GetQuoteClient(Creds.GetInstanceUrl());
    //    var response = await quoteClient.requestOrderAsync(uuid, quoteId);
    //    await Creds.Logout();
    //    return new BaseResponse { StatusCode = response.statusCode };
    //}

    [Action("Delete quote", Description = "Delete a Plunet quote")]
    public async Task DeleteQuote([ActionParameter] GetQuoteRequest request)
    {
        await ExecuteWithRetry(() => QuoteClient.deleteAsync(Uuid, ParseId(request.QuoteId)));
    }

    [Action("Update quote", Description = "Update Plunet quote")]
    public async Task<QuoteResponse> UpdateQuote([ActionParameter] GetQuoteRequest quote,
        [ActionParameter] CreateQuoteRequest request)
    {
        await ExecuteWithRetry(() => QuoteClient.updateAsync(Uuid, new QuoteIN
        {
            quoteID = ParseId(quote.QuoteId),
            projectName = request.ProjectName,
            customerID = ParseId(request.CustomerId),
            subject = request.Subject,
            creationDate = DateTime.Now,
            currency = request.Currency,
            projectManagerMemo = request.ProjectManagerMemo,
            referenceNumber = request.ReferenceNumber,
            status = ParseId(request.Status)
        }, false));

        if (request.ProjectStatus is not null)
            await ExecuteWithRetry(() => QuoteClient.setProjectStatusAsync(Uuid, ParseId(quote.QuoteId), ParseId(request.ProjectStatus)));

        if (request.ProjectManagerId is not null)
            await ExecuteWithRetry(() => QuoteClient.setProjectmanagerIDAsync(Uuid, ParseId(request.ProjectManagerId), ParseId(quote.QuoteId)));

        if (request.ProjectCategory is not null)
            await ExecuteWithRetry(() => QuoteClient.setProjectCategoryAsync(Uuid, request.ProjectCategory, Language, ParseId(quote.QuoteId)));

        return await GetQuote(quote);
    }

    [Action("Add language combination to quote", Description = "Add a new language combination to an existing quote")]
    public async Task<AddLanguageCombinationResponse> AddLanguageCombinationToQuote(
        [ActionParameter] GetQuoteRequest quote, [ActionParameter] LanguageCombinationRequest request)
    {
        var sourceLanguage = await GetLanguageFromIsoOrFolderOrName(request.SourceLanguageCode);
        var targetLanguage = await GetLanguageFromIsoOrFolderOrName(request.TargetLanguageCode);

        var result = await ExecuteWithRetry(() => QuoteClient.addLanguageCombinationAsync(Uuid, sourceLanguage.name, targetLanguage.name, ParseId(quote.QuoteId)));

        return new()
        {
            LanguageCombinationId = result.ToString()
        };
    }   
}