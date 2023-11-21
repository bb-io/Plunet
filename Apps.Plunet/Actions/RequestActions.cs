﻿using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Request.Request;
using Apps.Plunet.Models.Request.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
using Blackbird.Plugins.Plunet.DataRequest30Service;

namespace Apps.Plunet.Actions;

[ActionList]
public class RequestActions : PlunetInvocable
{
    public RequestActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Search requests", Description = "Search for specific requests based on specific criteria")]
    public async Task<ListRequestsResponse> SearchRequests([ActionParameter] SearchRequestsInput input)
    {
        var searchResult = await RequestClient.searchAsync(Uuid, new()
        {
            languageCode = input.LanguageCode ?? string.Empty,
            sourceLanguage = input.SourceLanguage,
            targetLanguage = input.TargetLanguage,
            requestStatus = IntParser.Parse(input.RequestStatus, nameof(input.RequestStatus)) ?? -1,
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
                    mainID = IntParser.Parse(input.MainId, nameof(input.MainId)) ?? -1,
                    customerEntryType = IntParser.Parse(input.CustomerEntryType, nameof(input.CustomerEntryType)) ?? -1
                }
                : default
        });

        if (searchResult.data is null)
            throw new(searchResult.statusMessage);

        var getRequestTasks = searchResult.data
            .Where(x => x.HasValue)
            .Select(x => GetRequest(x.Value.ToString()));

        return new(await Task.WhenAll(getRequestTasks));
    }

    [Action("Get request", Description = "Get details for a Plunet request")]
    public async Task<RequestResponse> GetRequest([ActionParameter] [Display("Request ID")] string requestId)
    {
        var intRequestId = IntParser.Parse(requestId, nameof(requestId))!.Value;
        var requestResult = await RequestClient.getRequestObjectAsync(Uuid, intRequestId);

        if (requestResult.data is null)
            throw new(requestResult.statusMessage);

        return new(requestResult.data);
    }

    [Action("Create request", Description = "Create a new request in Plunet")]
    public async Task<RequestResponse> CreateRequest([ActionParameter] CreatеRequestRequest request)
    {
        var requestIdResult = await RequestClient.insert2Async(Uuid, new()
        {
            briefDescription = request.BriefDescription,
            creationDate = DateTime.Now,
            deliveryDate = request.DeliveryDate ?? default,
            orderID = IntParser.Parse(request.OrderId, nameof(request.OrderId)) ?? default,
            subject = request.Subject,
            quotationDate = request.QuotationDate ?? default,
            status = IntParser.Parse(request.Status, nameof(request.Status)) ?? default,
            quoteID = IntParser.Parse(request.QuoteId, nameof(request.QuoteId)) ?? default
        });

        if (requestIdResult.statusMessage != ApiResponses.Ok)
            throw new(requestIdResult.statusMessage);

        var requestId = requestIdResult.data;

        if (request.Service is not null)
            await RequestClient.addServiceAsync(Uuid, request.Service, requestId);

        if (request.ContactId is not null)
            await RequestClient.setCustomerContactIDAsync(Uuid, requestId,
                IntParser.Parse(request.ContactId, nameof(request.ContactId))!.Value);

        if (request.CustomerId is not null)
            await RequestClient.setCustomerIDAsync(Uuid,
                IntParser.Parse(request.CustomerId, nameof(request.CustomerId))!.Value, requestId);

        if (request.ReferenceNumberOfPrev is not null)
            await RequestClient.setCustomerRefNo_PrevOrderAsync(Uuid, request.ReferenceNumberOfPrev, requestId);

        if (request.ReferenceNumber is not null)
            await RequestClient.setCustomerRefNoAsync(Uuid, request.ReferenceNumber, requestId);

        if (request.IsEn10538 is not null)
            await RequestClient.setEN15038RequestedAsync(Uuid, request.IsEn10538.Value, requestId);

        if (request.MasterProjectId is not null)
            await RequestClient.setMasterProjectIDAsync(Uuid, requestId,
                IntParser.Parse(request.MasterProjectId, nameof(request.MasterProjectId))!.Value);

        if (request.Price is not null)
            await RequestClient.setPriceAsync(Uuid, request.Price.Value, requestId);

        if (request.IsRushRequest is not null)
            await RequestClient.setRushRequestAsync(Uuid, request.IsRushRequest.Value, requestId);

        if (request.WordCount is not null)
            await RequestClient.setWordCountAsync(Uuid, request.WordCount.Value, requestId);

        if (request.WorkflowId is not null)
            await RequestClient.setWorkflowIDAsync(Uuid, requestId,
                IntParser.Parse(request.WorkflowId, nameof(request.WorkflowId))!.Value);

        return await GetRequest(requestId.ToString());
    }

    [Action("Update request", Description = "Update Plunet request")]
    public async Task<RequestResponse> UpdateRequest([ActionParameter] UpdateRequestRequest request)
    {
        var result = await RequestClient.updateAsync(Uuid, new RequestIN
        {
            requestID = IntParser.Parse(request.RequestId, nameof(request.RequestId))!.Value,
            briefDescription = request.BriefDescription,
            creationDate = DateTime.Now,
            deliveryDate = request.DeliveryDate ?? default,
            orderID = IntParser.Parse(request.OrderId, nameof(request.OrderId)) ?? default,
            subject = request.Subject,
            quotationDate = request.QuotationDate ?? default,
            status = IntParser.Parse(request.Status, nameof(request.Status)) ?? default,
            quoteID = IntParser.Parse(request.QuoteId, nameof(request.QuoteId)) ?? default
        }, false);

        if (result.statusMessage != ApiResponses.Ok)
            throw new(result.statusMessage);

        return await GetRequest(request.RequestId);
    }

    [Action("Delete request", Description = "Delete a Plunet request")]
    public async Task DeleteRequest([ActionParameter] [Display("Request ID")] string requestId)
    {
        var intRequestId = IntParser.Parse(requestId, nameof(requestId))!.Value;
        await RequestClient.deleteAsync(Uuid, intRequestId);
    }

    [Action("Add language combination to request", Description = "Add a language combination to the specific request")]
    public async Task AddLanguageCombination(
        [ActionParameter] [Display("Request ID")]
        string requestId,
        [ActionParameter] LanguagesRequest langs)
    {
        var intRequestId = IntParser.Parse(requestId, nameof(requestId))!.Value;
        var response = await RequestClient.addLanguageCombinationAsync(Uuid, langs.SourceLanguageCode,
            langs.TargetLanguageCode,
            intRequestId);

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);
    }
}