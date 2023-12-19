using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Quote.Request;
using Apps.Plunet.Models.Quote.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataQuote30Service;

namespace Apps.Plunet.Actions;

[ActionList]
public class QuoteActions : PlunetInvocable
{
    public QuoteActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Search quotes", Description = "Search for specific quotes based on specific criteria")]
    public async Task<ListQuotesResponse> SearchQuotes([ActionParameter] SearchQuotesInput input)
    {
        var searchResult = await QuoteClient.searchAsync(Uuid, new()
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
        });

        if (searchResult.statusMessage != ApiResponses.Ok)
            throw new(searchResult.statusMessage);

        if (searchResult.data is null)
            return new(Enumerable.Empty<QuoteResponse>());

        var getRequestTasks = searchResult.data
            .Where(x => x.HasValue)
            .Select(x => GetQuote(new GetQuoteRequest { QuoteId = x.Value.ToString() }));

        return new(await Task.WhenAll(getRequestTasks));
    }

    [Action("Get quote", Description = "Get details for a Plunet quote")]
    public async Task<QuoteResponse> GetQuote([ActionParameter] GetQuoteRequest request)
    {
        var quoteResult = await QuoteClient.getQuoteObjectAsync(Uuid, ParseId(request.QuoteId));

        if (quoteResult.data is null)
            throw new(quoteResult.statusMessage);

        var itemsResult = await ItemClient.getAllItemObjectsAsync(Uuid, ParseId(request.QuoteId), 1);

        if (itemsResult.statusMessage != ApiResponses.Ok)
            throw new(itemsResult.statusMessage);

        var totalPrice = itemsResult.data.Sum(x => x.totalPrice);
        
        return new(quoteResult.data)
        {
            TotalPrice = totalPrice
        };
    }

    [Action("Create quote", Description = "Create a new quote in Plunet, optionally using a template")]
    public async Task<QuoteResponse> CreateQuote([ActionParameter] QuoteTemplateRequest template, [ActionParameter] CreateQuoteRequest request)
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

        var quoteIdResult = template.TemplateId == null ? 
            await QuoteClient.insert2Async(Uuid,quoteIn) : 
            await QuoteClient.insert_byTemplateAsync(Uuid, quoteIn, ParseId(template.TemplateId));

        if (quoteIdResult.statusMessage != ApiResponses.Ok)
            throw new(quoteIdResult.statusMessage);

        var quoteId = quoteIdResult.data;

        if (request.RequestId is not null)
            await QuoteClient.setRequestIDAsync(Uuid, quoteId, ParseId(request.RequestId));

        if (request.ProjectStatus is not null)
            await QuoteClient.setProjectStatusAsync(Uuid, quoteId, ParseId(request.ProjectStatus));

        if (request.ProjectManagerId is not null)
            await QuoteClient.setProjectmanagerIDAsync(Uuid, ParseId(request.ProjectManagerId), quoteId);

        if (request.ExternalId is not null)
            await QuoteClient.setExternalIDAsync(Uuid, quoteId, request.ExternalId);

        if (request.ContactId is not null)
            await QuoteClient.setCustomerContactIDAsync(Uuid, ParseId(request.ContactId), quoteId);

        return await GetQuote(new GetQuoteRequest { QuoteId = quoteId.ToString() });
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
        await QuoteClient.deleteAsync(Uuid, ParseId(request.QuoteId));
    }

    [Action("Update quote", Description = "Update Plunet quote")]
    public async Task<QuoteResponse> UpdateQuote([ActionParameter] GetQuoteRequest quote, [ActionParameter] CreateQuoteRequest request)
    {
        var result = await QuoteClient.updateAsync(Uuid, new QuoteIN
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
        }, false);

        if (result.statusMessage != ApiResponses.Ok)
            throw new(result.statusMessage);

        return await GetQuote(quote);
    }

    [Action("Add language combination to quote", Description = "Add a new language combination to an existing quote")]
    public async Task<AddLanguageCombinationResponse> AddLanguageCombinationToQuote([ActionParameter] GetQuoteRequest quote, LanguageCombinationRequest request)
    {
        var sourceLanguage = await GetLanguageFromIsoOrFolderOrName(request.SourceLanguageCode);
        var targetLanguage = await GetLanguageFromIsoOrFolderOrName(request.TargetLanguageCode);

        var result = await QuoteClient.addLanguageCombinationAsync(Uuid, sourceLanguage.name, targetLanguage.name, ParseId(quote.QuoteId));

        return new()
        {
            LanguageCombinationId = result.data.ToString()
        };
    }
}