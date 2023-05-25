using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataAdmin30Service;
using Blackbird.Plugins.Plunet.DataQuote30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Order;
using Blackbird.Plugins.Plunet.Models.Quote;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class QuoteActions
{
    [Display("Quotes")]
    [Action("Get quote", Description = "Get details for a Plunet quote")]
    public async Task<QuoteResponse> GetQuote(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int quoteId)
    {
        var uuid = authProviders.GetAuthToken();
        using var quoteClient = Clients.GetQuoteClient(authProviders.GetInstanceUrl());
        var quoteResult = await quoteClient.getQuoteObjectAsync(uuid, quoteId);
        var response = quoteResult.data ?? null;
        await authProviders.Logout();
        return MapQuoteResponse(response);
    }

    [Display("Quotes")]
    [Action("Create quote", Description = "Create a new quote in Plunet")]
    public async Task<CreateQuoteResponse> CreateQuote(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] CreateQuoteRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        using var quoteClient = Clients.GetQuoteClient(authProviders.GetInstanceUrl());
        var quoteIdResult = await quoteClient.insert2Async(uuid, new QuoteIN
        {
            projectName = request.ProjectName,
            customerID = request.CustomerId,
            subject = request.ProjectName,
            creationDate = DateTime.Now,
        });
        await authProviders.Logout();
        return new CreateQuoteResponse { QuoteId = quoteIdResult.data};
    }

    //[Display("Quotes")]
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

    //[Display("Quotes")]
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

    //[Display("Quotes")]
    //[Action("Request order to quote", Description = "Request order to a Plunet quote")]
    //public async Task<BaseResponse> RequestOrder(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int quoteId)
    //{
    //    var uuid = authProviders.GetAuthToken();
    //    using var quoteClient = Clients.GetQuoteClient(authProviders.GetInstanceUrl());
    //    var response = await quoteClient.requestOrderAsync(uuid, quoteId);
    //    await authProviders.Logout();
    //    return new BaseResponse { StatusCode = response.statusCode };
    //}

    [Display("Quotes")]
    [Action("Delete quote", Description = "Delete a Plunet quote")]
    public async Task<BaseResponse> DeleteQuote(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int quoteId)
    {
        var uuid = authProviders.GetAuthToken();
        using var quoteClient = Clients.GetQuoteClient(authProviders.GetInstanceUrl());
        var response = await quoteClient.deleteAsync(uuid, quoteId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    private (string SourceLanguageName, string TargetLanguageName)
        GetLanguageNamesCombinationByLanguageCodeIso(string token, string sourceLanguageCode, string targetLanguageCode)
    {
        using var adminClient = new DataAdmin30Client();
        var availableLanguagesResult = adminClient.getAvailableLanguagesAsync(token, "en").GetAwaiter().GetResult();
        if (availableLanguagesResult.data == null || availableLanguagesResult.data.Length == 0)
        {
            return new ValueTuple<string, string>();
        }

        var sourceLanguage = availableLanguagesResult.data.FirstOrDefault(x =>
                                 x.isoCode.Equals(sourceLanguageCode, StringComparison.OrdinalIgnoreCase) ||
                                 x.folderName.Equals(sourceLanguageCode, StringComparison.OrdinalIgnoreCase)) ??
                             availableLanguagesResult.data.FirstOrDefault(x => x.isoCode.ToUpper() == "ENG");
        var targetLanguage = availableLanguagesResult.data.FirstOrDefault(x =>
            x.isoCode.Equals(targetLanguageCode, StringComparison.OrdinalIgnoreCase) ||
            x.folderName.Equals(targetLanguageCode, StringComparison.OrdinalIgnoreCase));
        return targetLanguage == null
            ? new ValueTuple<string, string>()
            : new ValueTuple<string, string>(sourceLanguage?.name, targetLanguage.name);
    }

    private QuoteResponse MapQuoteResponse(Quote? quote)
    {
        return quote == null
            ? new QuoteResponse()
            : new QuoteResponse
            {
                Currency = quote.currency,
                ProjectName = quote.projectName,
                Rate = quote.rate
            };
    }
}