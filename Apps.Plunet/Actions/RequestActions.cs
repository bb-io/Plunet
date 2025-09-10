using Apps.Plunet.Constants;
using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Order;
using Apps.Plunet.Models.Quote.Request;
using Apps.Plunet.Models.Quote.Response;
using Apps.Plunet.Models.Request.Request;
using Apps.Plunet.Models.Request.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataRequest30Service;

namespace Apps.Plunet.Actions;

[ActionList("Requests")]
public class RequestActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{
    [Action("Search requests", Description = "Search for specific requests based on specific criteria")]
    public async Task<SearchResponse<RequestResponse>> SearchRequests([ActionParameter] SearchRequestsInput input)
    {
        var result = await ExecuteWithRetryAcceptNull(() => RequestClient.searchAsync(Uuid, new()
        {
            sourceLanguage = input.SourceLanguage ?? string.Empty,
            targetLanguage = input.TargetLanguage ?? string.Empty,
            requestStatus = ParseId(input.RequestStatus),
            timeFrame = input.DateFrom is not null || input.DateTo is not null
                ? new()
                {
                    dateFrom = input.DateFrom ?? default,
                    dateTo = input.DateTo ?? default
                }
                : default,
            SelectionEntry_Customer = input.MainId is not null || input.CustomerEntryType is not null
                ? new()
                {
                    mainID = ParseId(input.MainId),
                    customerEntryType = ParseId(input.CustomerEntryType)
                }
                : default
        }));

        if (result is null)
        {
            return new();
        }

        var ids = result
       .Where(x => x.HasValue)
       .Select(x => x!.Value)
       .ToArray();

        if (ids.Length == 0) return new();

        var limitedIds = ids.Take(input.Limit ?? SystemConsts.SearchLimit).ToArray();

        if (input.OnlyReturnIds == true)
        {
            var idOnly = limitedIds
                .Select(id => new RequestResponse(
                    new Request
                    {
                        requestID = id,
                        briefDescription = string.Empty,
                        subject = string.Empty
                    },
                    customerId: string.Empty,
                    requestNumber: string.Empty,
                    customerRefNo: string.Empty,
                    customerContactId: string.Empty
                ))
                .ToList();

            return new SearchResponse<RequestResponse>(idOnly);
        }

        var responses = new List<RequestResponse>(limitedIds.Length);
        foreach (var id in limitedIds)
        {
            var r = await GetRequest(id.ToString());
            responses.Add(r);
        }

        return new SearchResponse<RequestResponse>(responses);
    }

    [Action("Find request", Description = "Find a specific request based on specific criteria")]
    public async Task<FindResponse<RequestResponse>> FindRequest([ActionParameter] SearchRequestsInput input)
    {
        var searchResult = await SearchRequests(input);
        return new(searchResult.Items.FirstOrDefault(), searchResult.TotalCount);
    }

    [Action("Get request", Description = "Get details for a Plunet request")]
    public async Task<RequestResponse> GetRequest([ActionParameter][Display("Request ID")] string requestId)
    {
        var request = await ExecuteWithRetry(() => RequestClient.getRequestObjectAsync(Uuid, ParseId(requestId)));
        var customerId = await ExecuteWithRetry(() => RequestClient.getCustomerIDAsync(Uuid, request.requestID));
        var requestNumber = await ExecuteWithRetry(() => RequestClient.getRequestNo_for_ViewAsync(Uuid, request.requestID));
        
        var customerContactId = await ExecuteWithRetry(() => RequestClient.getCustomerContactIDAsync(Uuid, request.requestID));
        var customerRefNo = await ExecuteWithRetry(() => RequestClient.getCustomerRefNoAsync(Uuid, request.requestID));
        
        return new RequestResponse(
        request,
        customerId.ToString(),
        requestNumber,
        customerRefNo,
        customerContactId.ToString());
    }

    [Action("Create request", Description = "Create a new request in Plunet")]
    public async Task<RequestResponse> CreateRequest([ActionParameter] CreatеRequestRequest request)
    {
        var requestId = await ExecuteWithRetry(() => RequestClient.insert2Async(Uuid, new()
        {
            briefDescription = request.BriefDescription,
            creationDate = DateTime.Now,
            deliveryDate = request.DeliveryDate ?? default,
            orderID = ParseId(request.OrderId),
            subject = request.Subject,
            quotationDate = request.QuotationDate ?? default,
            status = ParseId(request.Status),
            quoteID = ParseId(request.QuoteId)
        }));

        if (request.Service is not null)
            await ExecuteWithRetry(() => RequestClient.addServiceAsync(Uuid, request.Service, requestId));

        if (request.ContactId is not null)
            await ExecuteWithRetry(() => RequestClient.setCustomerContactIDAsync(Uuid, requestId, ParseId(request.ContactId)));

        if (request.CustomerId is not null)
            await ExecuteWithRetry(() => RequestClient.setCustomerIDAsync(Uuid, ParseId(request.CustomerId), requestId));

        if (request.ReferenceNumberOfPrev is not null)
            await ExecuteWithRetry(() => RequestClient.setCustomerRefNo_PrevOrderAsync(Uuid, request.ReferenceNumberOfPrev, requestId));

        if (request.ReferenceNumber is not null)
            await ExecuteWithRetry(() => RequestClient.setCustomerRefNoAsync(Uuid, request.ReferenceNumber, requestId));

        if (request.IsEn10538 is not null)
            await ExecuteWithRetry(() => RequestClient.setEN15038RequestedAsync(Uuid, request.IsEn10538.Value, requestId));

        if (request.MasterProjectId is not null)
            await ExecuteWithRetry(() => RequestClient.setMasterProjectIDAsync(Uuid, requestId, ParseId(request.MasterProjectId)));

        if (request.Price is not null)
            await ExecuteWithRetry(() => RequestClient.setPriceAsync(Uuid, request.Price.Value, requestId));

        if (request.IsRushRequest is not null)
            await ExecuteWithRetry(() => RequestClient.setRushRequestAsync(Uuid, request.IsRushRequest.Value, requestId));

        if (request.WordCount is not null)
            await ExecuteWithRetry(() => RequestClient.setWordCountAsync(Uuid, request.WordCount.Value, requestId));

        if (request.WorkflowId is not null)
            await ExecuteWithRetry(() => RequestClient.setWorkflowIDAsync(Uuid, requestId, ParseId(request.WorkflowId)));

        return await GetRequest(requestId.ToString());
    }

    [Action("Update request", Description = "Update Plunet request")]
    public async Task<RequestResponse> UpdateRequest([ActionParameter] UpdateRequestRequest request)
    {
        await ExecuteWithRetry(() => RequestClient.updateAsync(Uuid, new RequestIN
        {
            requestID = ParseId(request.RequestId),
            briefDescription = request.BriefDescription,
            creationDate = DateTime.Now,
            deliveryDate = request.DeliveryDate ?? default,
            orderID = ParseId(request.OrderId),
            subject = request.Subject,
            quotationDate = request.QuotationDate ?? default,
            status = ParseId(request.Status),
            quoteID = ParseId(request.QuoteId)
        }, false));

        return await GetRequest(request.RequestId);
    }

    [Action("Delete request", Description = "Delete a Plunet request")]
    public async Task DeleteRequest([ActionParameter][Display("Request ID")] string requestId)
    {
        await ExecuteWithRetry(() => RequestClient.deleteAsync(Uuid, ParseId(requestId)));
    }

    [Action("Create quote from request", Description = "Create quote from request")]
    public async Task<QuoteResponse> CreateQuoteFromRequest([ActionParameter][Display("Request ID")] string requestId,
        [ActionParameter][Display("Request ID")] QuoteTemplateRequest quoteTemplateID)
    {
        var result = await ExecuteWithRetry(() =>
            RequestClient.quoteRequest_byTemplateAsync(Uuid, ParseId(requestId), quoteTemplateID?.TemplateId is null ? 0 : ParseId(quoteTemplateID.TemplateId)));
        var actions = new QuoteActions(invocationContext);
        return await actions.GetQuote(new GetQuoteRequest { QuoteId = result.ToString() });
    }

    [Action("Create order from request", Description = "Create quote from request")]
    public async Task<OrderResponse> CreateOrderFromRequest([ActionParameter][Display("Request ID")] string requestId, [ActionParameter, Display("Template"), DataSource(typeof(TemplateDataHandler))]
        string? templateId)
    {
        var result = await ExecuteWithRetry(() => RequestClient.orderRequest_byTemplateAsync(Uuid, ParseId(requestId), templateId is null ? 0 : ParseId(templateId)));
        var actions = new OrderActions(invocationContext);
        return await actions.GetOrder(new OrderRequest { OrderId = result.ToString() });
    }

    [Action("Add language combination to request", Description = "Add a language combination to the specific request")]
    public async Task AddLanguageCombination(
        [ActionParameter] [Display("Request ID")]
        string requestId,
        [ActionParameter] LanguagesRequest langs)
    {
        await ExecuteWithRetry(() => RequestClient.addLanguageCombinationAsync(Uuid, langs.SourceLanguageCode, langs.TargetLanguageCode, ParseId(requestId)));
    }
}

