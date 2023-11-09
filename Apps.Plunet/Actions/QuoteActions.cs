using Apps.Plunet.Api;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Quote;
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
    
    [Action("Get quote", Description = "Get details for a Plunet quote")]
    public async Task<QuoteResponse> GetQuote([ActionParameter] [Display("Quote ID")] string quoteId)
    {
        var intQuoteId = IntParser.Parse(quoteId, nameof(quoteId))!.Value;
        var uuid = Creds.GetAuthToken();
        
        await using var quoteClient = Clients.GetQuoteClient(Creds.GetInstanceUrl());
        var quoteResult = await quoteClient.getQuoteObjectAsync(uuid, intQuoteId);
       
        await Creds.Logout();

        if (quoteResult.data is null)
            throw new(quoteResult.statusMessage);
        
        return new(quoteResult.data);
    }

    [Action("Create quote", Description = "Create a new quote in Plunet")]
    public async Task<CreateQuoteResponse> CreateQuote([ActionParameter] CreateQuoteRequest request)
    {
        var uuid = Creds.GetAuthToken();
        
        await using var quoteClient = Clients.GetQuoteClient(Creds.GetInstanceUrl());
        var quoteIdResult = await quoteClient.insert2Async(uuid, new QuoteIN
        {
            projectName = request.ProjectName,
            customerID = IntParser.Parse(request.CustomerId, nameof(request.CustomerId)) ?? default,
            subject = request.Subject,
            creationDate = DateTime.Now,
            currency = request.Currency,
            projectManagerMemo = request.ProjectManagerMemo,
            referenceNumber = request.ReferenceNumber,
            status = request.Status ?? default
        });
       
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
        
        await using var quoteClient = Clients.GetQuoteClient(Creds.GetInstanceUrl());
        await quoteClient.deleteAsync(uuid, intQuoteId);
        
        await Creds.Logout();
    }

    [Action("Update quote", Description = "Update Plunet quote")]
    public async Task UpdateQuote([ActionParameter] UpdateQuoteRequest request)
    {
        var uuid = Creds.GetAuthToken();
        
        var quoteClient = Clients.GetQuoteClient(Creds.GetInstanceUrl());
        await quoteClient.updateAsync(uuid, new QuoteIN
        {
            quoteID = IntParser.Parse(request.QuoteId, nameof(request.QuoteId))!.Value,
            projectName = request.ProjectName,
            customerID = IntParser.Parse(request.CustomerId, nameof(request.CustomerId)) ?? default,
            subject = request.Subject,
            creationDate = DateTime.Now,
            currency = request.Currency,
            projectManagerMemo = request.ProjectManagerMemo,
            referenceNumber = request.ReferenceNumber,
            status = request.Status ?? default
        }, false);
        
        await Creds.Logout();
    }
}