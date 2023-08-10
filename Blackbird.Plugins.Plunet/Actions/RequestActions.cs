using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataRequest30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Request;
using Blackbird.Plugins.Plunet.Utils;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class RequestActions
{
    [Action("Get request", Description = "Get details for a Plunet request")]
    public async Task<RequestResponse> GetRequest(
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] [Display("Request ID")]
        string requestId)
    {
        var intRequestId = IntParser.Parse(requestId, nameof(requestId))!.Value;
        var uuid = authProviders.GetAuthToken();

        await using var requestClient = Clients.GetRequestClient(authProviders.GetInstanceUrl());
        var requestResult = await requestClient.getRequestObjectAsync(uuid, intRequestId);

        await authProviders.Logout();

        if (requestResult.data is null)
            throw new(requestResult.statusMessage);

        return new(requestResult.data);
    }

    [Action("Create request", Description = "Create a new request in Plunet")]
    public async Task<CreatеRequestResponse> CreateRequest(
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] CreatеRequestRequest request)
    {
        var uuid = authProviders.GetAuthToken();

        await using var requestClient = Clients.GetRequestClient(authProviders.GetInstanceUrl());
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

        await authProviders.Logout();

        return new CreatеRequestResponse { RequestId = requestIdResult.data.ToString() };
    }

    [Action("Update request", Description = "Update Plunet request")]
    public async Task<BaseResponse> UpdateRequest(
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] UpdateRequestRequest request)
    {
        var uuid = authProviders.GetAuthToken();

        var requestClient = Clients.GetRequestClient(authProviders.GetInstanceUrl());
        var response = await requestClient.updateAsync(uuid, new RequestIN
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

        await authProviders.Logout();

        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Delete request", Description = "Delete a Plunet request")]
    public async Task<BaseResponse> DeleteRequest(
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] [Display("Request ID")]
        string requestId)
    {
        var intRequestId = IntParser.Parse(requestId, nameof(requestId))!.Value;
        var uuid = authProviders.GetAuthToken();

        await using var requestClient = Clients.GetRequestClient(authProviders.GetInstanceUrl());
        var response = await requestClient.deleteAsync(uuid, intRequestId);
        await authProviders.Logout();

        return new BaseResponse { StatusCode = response.statusCode };
    }
}