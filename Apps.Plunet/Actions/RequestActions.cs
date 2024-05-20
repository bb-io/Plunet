using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Request.Request;
using Apps.Plunet.Models.Request.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataRequest30Service;

namespace Apps.Plunet.Actions;

[ActionList]
public class RequestActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{
    [Action("Search requests", Description = "Search for specific requests based on specific criteria")]
    public async Task<ListRequestsResponse> SearchRequests([ActionParameter] SearchRequestsInput input)
    {
        var searchResult = await ExecuteWithRetry<IntegerArrayResult>(async () => await RequestClient.searchAsync(Uuid, new()
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

        if (searchResult.statusMessage != ApiResponses.Ok)
            throw new(searchResult.statusMessage);

        if (searchResult.data is null)
            return new(Enumerable.Empty<RequestResponse>());

        var results = new List<RequestResponse>();
        foreach (var id in searchResult.data.Where(x => x.HasValue).Take(input.Limit ?? SystemConsts.SearchLimit))
        {
            var requestResponse = await GetRequest(id.Value.ToString());
            results.Add(requestResponse);
        }

        return new(results);
    }

    [Action("Find request", Description = "Find a specific request based on specific criteria")]
    public async Task<RequestResponse?> FindRequest([ActionParameter] SearchRequestsInput input)
    {
        var searchResult = await SearchRequests(input);
        return searchResult.Requests.FirstOrDefault();
    }
    
    [Action("Get request", Description = "Get details for a Plunet request")]
    public async Task<RequestResponse> GetRequest([ActionParameter] [Display("Request ID")] string requestId)
    {
        var requestResult = await ExecuteWithRetry<RequestResult>(async () => await RequestClient.getRequestObjectAsync(Uuid, ParseId(requestId)));

        if (requestResult.data is null)
            throw new(requestResult.statusMessage);

        return new(requestResult.data);
    }

    [Action("Create request", Description = "Create a new request in Plunet")]
    public async Task<RequestResponse> CreateRequest([ActionParameter] CreatеRequestRequest request)
    {
        var requestIdResult = await ExecuteWithRetry<IntegerResult>(async () => await RequestClient.insert2Async(Uuid, new()
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

        if (requestIdResult.statusMessage != ApiResponses.Ok)
            throw new(requestIdResult.statusMessage);

        var requestId = requestIdResult.data;

        if (request.Service is not null)
            await ExecuteWithRetry<Result>(async () => await RequestClient.addServiceAsync(Uuid, request.Service, requestId));

        if (request.ContactId is not null)
            await ExecuteWithRetry<IntegerArrayResult>(async () => await RequestClient.setCustomerContactIDAsync(Uuid, requestId, ParseId(request.ContactId)));

        if (request.CustomerId is not null)
            await ExecuteWithRetry<IntegerArrayResult>(async () => await RequestClient.setCustomerIDAsync(Uuid, ParseId(request.CustomerId), requestId));

        if (request.ReferenceNumberOfPrev is not null)
            await ExecuteWithRetry<IntegerArrayResult>(async () => await RequestClient.setCustomerRefNo_PrevOrderAsync(Uuid, request.ReferenceNumberOfPrev, requestId));

        if (request.ReferenceNumber is not null)
            await ExecuteWithRetry<IntegerArrayResult>(async () => await RequestClient.setCustomerRefNoAsync(Uuid, request.ReferenceNumber, requestId));

        if (request.IsEn10538 is not null)
            await ExecuteWithRetry<IntegerArrayResult>(async () => await RequestClient.setEN15038RequestedAsync(Uuid, request.IsEn10538.Value, requestId));

        if (request.MasterProjectId is not null)
            await ExecuteWithRetry<IntegerArrayResult>(async () => await RequestClient.setMasterProjectIDAsync(Uuid, requestId, ParseId(request.MasterProjectId)));

        if (request.Price is not null)
            await ExecuteWithRetry<IntegerArrayResult>(async () => await RequestClient.setPriceAsync(Uuid, request.Price.Value, requestId));

        if (request.IsRushRequest is not null)
            await ExecuteWithRetry<IntegerArrayResult>(async () => await RequestClient.setRushRequestAsync(Uuid, request.IsRushRequest.Value, requestId));

        if (request.WordCount is not null)
            await ExecuteWithRetry<IntegerArrayResult>(async () => await RequestClient.setWordCountAsync(Uuid, request.WordCount.Value, requestId));

        if (request.WorkflowId is not null)
            await ExecuteWithRetry<IntegerArrayResult>(async () => await RequestClient.setWorkflowIDAsync(Uuid, requestId, ParseId(request.WorkflowId)));

        return await GetRequest(requestId.ToString());
    }

    [Action("Update request", Description = "Update Plunet request")]
    public async Task<RequestResponse> UpdateRequest([ActionParameter] UpdateRequestRequest request)
    {
        var result = await ExecuteWithRetry<Result>(async () => await RequestClient.updateAsync(Uuid, new RequestIN
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

        if (result.statusMessage != ApiResponses.Ok)
            throw new(result.statusMessage);

        return await GetRequest(request.RequestId);
    }

    [Action("Delete request", Description = "Delete a Plunet request")]
    public async Task DeleteRequest([ActionParameter] [Display("Request ID")] string requestId)
    {
        await ExecuteWithRetry<Result>(async () => await RequestClient.deleteAsync(Uuid, ParseId(requestId)));
    }

    [Action("Add language combination to request", Description = "Add a language combination to the specific request")]
    public async Task AddLanguageCombination(
        [ActionParameter] [Display("Request ID")]
        string requestId,
        [ActionParameter] LanguagesRequest langs)
    {
        var response = await ExecuteWithRetry<Result>(async () => await RequestClient.addLanguageCombinationAsync(Uuid, langs.SourceLanguageCode, langs.TargetLanguageCode, ParseId(requestId)));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);
    }
    
    private async Task<T> ExecuteWithRetry<T>(Func<Task<Result>> func, int maxRetries = 10, int delay = 1000)
        where T : Result
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();
            
            if(result.statusMessage == ApiResponses.Ok)
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