using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataQuote30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Quote;
using Blackbird.Plugins.Plunet.Utils;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class QuoteActions
{
    [Action("Get quote", Description = "Get details for a Plunet quote")]
    public async Task<QuoteResponse> GetQuote(
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] [Display("Quote ID")] string quoteId)
    {
        var intQuoteId = IntParser.Parse(quoteId, nameof(quoteId))!.Value;
        var uuid = authProviders.GetAuthToken();
        
        await using var quoteClient = Clients.GetQuoteClient(authProviders.GetInstanceUrl());
        var quoteResult = await quoteClient.getQuoteObjectAsync(uuid, intQuoteId);
        await authProviders.Logout();

        if (quoteResult.data is null)
            throw new(quoteResult.statusMessage);
        
        return new(quoteResult.data);
    }

    [Action("Create quote", Description = "Create a new quote in Plunet")]
    public async Task<CreateQuoteResponse> CreateQuote(
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] CreateQuoteRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        
        await using var quoteClient = Clients.GetQuoteClient(authProviders.GetInstanceUrl());
        var quoteIdResult = await quoteClient.insert2Async(uuid, new QuoteIN
        {
            projectName = request.ProjectName,
            customerID = IntParser.Parse(request.CustomerId, nameof(request.CustomerId)) ?? default,
            subject = request.ProjectName,
            creationDate = DateTime.Now,
            currency = request.Currency,
            projectManagerMemo = request.ProjectManagerMemo,
            referenceNumber = request.ReferenceNumber,
            status = request.Status ?? default
        });
       
        await authProviders.Logout();
        
        return new CreateQuoteResponse { QuoteId = quoteIdResult.data.ToString() };
    }

    //[Action("Create quote based on template", Description = "Create a new quote based on a template")]
    //public async Task<CreateQuoteResponse> CreateQuoteBasedOnTemplate(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] CreateQuoteRequest request,
    //    [ActionParameter] string templateName)
    //{
    //    var uuid = authProviders.GetAuthToken();
    //    using var quoteClient = Clients.GetQuoteClient(authProviders.GetInstanceUrl());
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
    //        await authProviders.Logout();
    //        return new CreateQuoteResponse();
    //    }

    //    var template = templates.data.FirstOrDefault(t =>
    //        t.templateName.Contains(templateName, StringComparison.OrdinalIgnoreCase));
    //    if (template == null)
    //    {
    //        await authProviders.Logout();
    //        return new CreateQuoteResponse();
    //    }

    //    var quoteIdResult = await quoteClient.insert_byTemplateAsync(uuid, quote, template.templateID);
    //    await authProviders.Logout();
    //    return new CreateQuoteResponse { QuoteId = quoteIdResult.data };
    //}

    //[Action("Add language combination to quote", Description = "Add a new language combination to an existing quote")]
    //public async Task<AddLanguageCombinationResponse> AddLanguageCombinationToQuote(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] AddLanguageCombinationToQuoteRequest request)
    //{
    //    var uuid = authProviders.GetAuthToken();
    //    using var quoteClient = Clients.GetQuoteClient(authProviders.GetInstanceUrl());
    //    var langCombination =
    //        GetLanguageNamesCombinationByLanguageCodeIso(uuid, request.SourceLanguageCode,
    //            request.TargetLanguageCode);
    //    if (string.IsNullOrEmpty(langCombination.TargetLanguageName))
    //    {
    //        await authProviders.Logout();
    //        return new AddLanguageCombinationResponse();
    //    }

    //    var result = await quoteClient.addLanguageCombinationAsync(uuid, langCombination.SourceLanguageName,
    //        langCombination.TargetLanguageName, request.QuoteId);
    //    await authProviders.Logout();
    //    return new AddLanguageCombinationResponse { LanguageCombinationId = result.data };
    //}

    //[Action("Request order to quote", Description = "Request order to a Plunet quote")]
    //public async Task<BaseResponse> RequestOrder(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int quoteId)
    //{
    //    var uuid = authProviders.GetAuthToken();
    //    using var quoteClient = Clients.GetQuoteClient(authProviders.GetInstanceUrl());
    //    var response = await quoteClient.requestOrderAsync(uuid, quoteId);
    //    await authProviders.Logout();
    //    return new BaseResponse { StatusCode = response.statusCode };
    //}

    [Action("Delete quote", Description = "Delete a Plunet quote")]
    public async Task<BaseResponse> DeleteQuote(
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] [Display("Quote ID")] string quoteId)
    {
        var intQuoteId = IntParser.Parse(quoteId, nameof(quoteId))!.Value;
        var uuid = authProviders.GetAuthToken();
        
        await using var quoteClient = Clients.GetQuoteClient(authProviders.GetInstanceUrl());
        var response = await quoteClient.deleteAsync(uuid, intQuoteId);
        await authProviders.Logout();
        
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Update quote", Description = "Update Plunet quote")]
    public async Task<BaseResponse> UpdateQuote(
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] UpdateQuoteRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        
        var quoteClient = Clients.GetQuoteClient(authProviders.GetInstanceUrl());
        var response = await quoteClient.updateAsync(uuid, new QuoteIN
        {
            quoteID = IntParser.Parse(request.QuoteId, nameof(request.QuoteId))!.Value,
            projectName = request.ProjectName,
            customerID = IntParser.Parse(request.CustomerId, nameof(request.CustomerId)) ?? default,
            subject = request.ProjectName,
            creationDate = DateTime.Now,
            currency = request.Currency,
            projectManagerMemo = request.ProjectManagerMemo,
            referenceNumber = request.ReferenceNumber,
            status = request.Status ?? default
        }, false);
        
        await authProviders.Logout();
        
        return new BaseResponse { StatusCode = response.statusCode };
    }
}