using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Quote;
using Apps.Plunet.Models.Quote.Request;
using Apps.Plunet.Models.Quote.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
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
        var uuid = Creds.GetAuthToken();

        var searchResult = await QuoteClient.searchAsync(uuid, new()
        {
            languageCode = input.LanguageCode ?? string.Empty,
            sourceLanguage = input.SourceLanguage,
            targetLanguage = input.TargetLanguage,
            quoteStatus = IntParser.Parse(input.QuoteStatus, nameof(input.QuoteStatus)) ?? -1,
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
                    mainID = IntParser.Parse(input.CustomerMainId, nameof(input.CustomerMainId)) ?? -1,
                    customerEntryType = IntParser.Parse(input.CustomerEntryType, nameof(input.CustomerEntryType)) ?? -1
                }
                : default,
            SelectionEntry_Resource = input.ResourceMainId is not null || input.ResourceEntryType is not null
                ? new()
                {
                    mainID = IntParser.Parse(input.ResourceMainId, nameof(input.ResourceMainId)) ?? -1,
                    resourceEntryType = IntParser.Parse(input.ResourceEntryType, nameof(input.ResourceEntryType)) ?? -1
                }
                : default
        });

        if (searchResult.data is null)
            throw new(searchResult.statusMessage);

        var getRequestTasks = searchResult.data
            .Where(x => x.HasValue)
            .Select(x => GetQuote(x.Value.ToString()));

        return new(await Task.WhenAll(getRequestTasks));
    }

    [Action("Get quote", Description = "Get details for a Plunet quote")]
    public async Task<QuoteResponse> GetQuote([ActionParameter] [Display("Quote ID")] string quoteId)
    {
        var intQuoteId = IntParser.Parse(quoteId, nameof(quoteId))!.Value;
        var uuid = Creds.GetAuthToken();

        var quoteResult = await QuoteClient.getQuoteObjectAsync(uuid, intQuoteId);

        if (quoteResult.data is null)
            throw new(quoteResult.statusMessage);

        return new(quoteResult.data);
    }

    [Action("Create quote", Description = "Create a new quote in Plunet")]
    public async Task<CreateQuoteResponse> CreateQuote([ActionParameter] CreateQuoteRequest request)
    {
        var uuid = Creds.GetAuthToken();

        var quoteIdResult = await QuoteClient.insert2Async(uuid, new QuoteIN
        {
            projectName = request.ProjectName,
            customerID = IntParser.Parse(request.CustomerId, nameof(request.CustomerId)) ?? default,
            subject = request.Subject,
            creationDate = DateTime.Now,
            currency = request.Currency,
            projectManagerMemo = request.ProjectManagerMemo,
            referenceNumber = request.ReferenceNumber,
            status = IntParser.Parse(request.Status, nameof(request.Status)) ?? default
        });

        var quoteId = quoteIdResult.data;

        if (request.RequestId is not null)
            await QuoteClient.setRequestIDAsync(uuid, quoteId,
                IntParser.Parse(request.RequestId, nameof(request.RequestId))!.Value);

        if (request.ProjectStatus is not null)
            await QuoteClient.setProjectStatusAsync(uuid, quoteId,
                IntParser.Parse(request.ProjectStatus, nameof(request.ProjectStatus))!.Value);

        if (request.ProjectManagerId is not null)
            await QuoteClient.setProjectmanagerIDAsync(uuid,
                IntParser.Parse(request.ProjectManagerId, nameof(request.ProjectManagerId))!.Value, quoteId);

        if (request.ExternalId is not null)
            await QuoteClient.setExternalIDAsync(uuid, quoteId, request.ExternalId);

        if (request.ContactId is not null)
            await QuoteClient.setCustomerContactIDAsync(uuid,
                IntParser.Parse(request.ContactId, nameof(request.ContactId))!.Value, quoteId);

        await Creds.Logout();

        return new()
        {
            QuoteId = quoteIdResult.data.ToString()
        };
    }

    //[Action("Create quote based on template", Description = "Create a new quote based on a template")]
    //public async Task<CreateQuoteResponse> CreateQuoteBasedOnTemplate(IEnumerable<AuthenticationCredentialsProvider> Creds, [ActionParameter] CreateQuoteRequest request,
    //    [ActionParameter] string templateName)
    //{
    //    var uuid = Creds.GetAuthToken();
    //    using var quoteClient = Clients.GetQuoteClient(Creds.GetInstanceUrl());
    //    var quote = new QuoteIN
    //    {
    //        projectName = request.ProjectName,
    //        customerID = request.CustomerId,
    //        subject = request.ProjectName,
    //        creationDate = DateTime.Now,
    //    };
    //    var templates = await quoteClient.getTemplateListAsync(uuid);
    //    if (templates == null || !templates.data.Any())
    //    {
    //        await Creds.Logout();
    //        return new CreateQuoteResponse();
    //    }

    //    var template = templates.data.FirstOrDefault(t =>
    //        t.templateName.Contains(templateName, StringComparison.OrdinalIgnoreCase));
    //    if (template == null)
    //    {
    //        await Creds.Logout();
    //        return new CreateQuoteResponse();
    //    }

    //    var quoteIdResult = await quoteClient.insert_byTemplateAsync(uuid, quote, template.templateID);
    //    await Creds.Logout();
    //    return new CreateQuoteResponse { QuoteId = quoteIdResult.data };
    //}

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
    public async Task DeleteQuote([ActionParameter] [Display("Quote ID")] string quoteId)
    {
        var intQuoteId = IntParser.Parse(quoteId, nameof(quoteId))!.Value;
        var uuid = Creds.GetAuthToken();

        await QuoteClient.deleteAsync(uuid, intQuoteId);

        await Creds.Logout();
    }

    [Action("Update quote", Description = "Update Plunet quote")]
    public async Task UpdateQuote([ActionParameter] UpdateQuoteRequest request)
    {
        var uuid = Creds.GetAuthToken();

        await QuoteClient.updateAsync(uuid, new QuoteIN
        {
            quoteID = IntParser.Parse(request.QuoteId, nameof(request.QuoteId))!.Value,
            projectName = request.ProjectName,
            customerID = IntParser.Parse(request.CustomerId, nameof(request.CustomerId)) ?? default,
            subject = request.Subject,
            creationDate = DateTime.Now,
            currency = request.Currency,
            projectManagerMemo = request.ProjectManagerMemo,
            referenceNumber = request.ReferenceNumber,
            status = IntParser.Parse(request.Status, nameof(request.Status)) ?? default
        }, false);

        await Creds.Logout();
    }
}