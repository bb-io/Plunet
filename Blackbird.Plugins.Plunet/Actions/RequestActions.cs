﻿using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
using Blackbird.Plugins.Plunet.DataRequest30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Invocables;
using Blackbird.Plugins.Plunet.Models.Request;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class RequestActions : PlunetInvocable
{
    public RequestActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Get request", Description = "Get details for a Plunet request")]
    public async Task<RequestResponse> GetRequest([ActionParameter] [Display("Request ID")] string requestId)
    {
        var intRequestId = IntParser.Parse(requestId, nameof(requestId))!.Value;
        var uuid = Creds.GetAuthToken();

        await using var requestClient = Clients.GetRequestClient(Creds.GetInstanceUrl());
        var requestResult = await requestClient.getRequestObjectAsync(uuid, intRequestId);

        await Creds.Logout();

        if (requestResult.data is null)
            throw new(requestResult.statusMessage);

        return new(requestResult.data);
    }

    [Action("Create request", Description = "Create a new request in Plunet")]
    public async Task<CreatеRequestResponse> CreateRequest([ActionParameter] CreatеRequestRequest request)
    {
        var uuid = Creds.GetAuthToken();

        await using var requestClient = Clients.GetRequestClient(Creds.GetInstanceUrl());
        var requestIdResult = await requestClient.insert2Async(uuid, new()
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

        await Creds.Logout();

        return new()
        {
            RequestId = requestIdResult.data.ToString()
        };
    }

    [Action("Update request", Description = "Update Plunet request")]
    public async Task UpdateRequest([ActionParameter] UpdateRequestRequest request)
    {
        var uuid = Creds.GetAuthToken();

        var requestClient = Clients.GetRequestClient(Creds.GetInstanceUrl());
        await requestClient.updateAsync(uuid, new RequestIN
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

        await Creds.Logout();
    }

    [Action("Delete request", Description = "Delete a Plunet request")]
    public async Task DeleteRequest([ActionParameter] [Display("Request ID")] string requestId)
    {
        var intRequestId = IntParser.Parse(requestId, nameof(requestId))!.Value;
        var uuid = Creds.GetAuthToken();

        await using var requestClient = Clients.GetRequestClient(Creds.GetInstanceUrl());
        await requestClient.deleteAsync(uuid, intRequestId);
       
        await Creds.Logout();
    }
}