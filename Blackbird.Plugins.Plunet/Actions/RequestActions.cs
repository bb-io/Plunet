using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataAdmin30Service;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.DataQuote30Service;
using Blackbird.Plugins.Plunet.DataRequest30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Customer;
using Blackbird.Plugins.Plunet.Models.Request;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class RequestActions
{
    [Action("Get request", Description = "Get details for a Plunet request")]
    public async Task<RequestResponse> GetQuote(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int requestId)
    {
        var uuid = authProviders.GetAuthToken();
        using var requestClient = Clients.GetRequestClient(authProviders.GetInstanceUrl());
        var requestResult = await requestClient.getRequestObjectAsync(uuid, requestId);
        var response = requestResult.data ?? null;
        await authProviders.Logout();
        return MapRequestResponse(response);
    }

    [Action("Create request", Description = "Create a new request in Plunet")]
    public async Task<CreatеRequestResponse> CreateQuote(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] CreatеRequestRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        using var requestClient = Clients.GetRequestClient(authProviders.GetInstanceUrl());
        var requestIdResult = await requestClient.insert2Async(uuid, new RequestIN
        {
            briefDescription = request.BriefDescription,
            creationDate = DateTime.Now,
            deliveryDate = request.DeliveryDate,
            orderID = request.OrderId,
            subject = request.Subject,
            quotationDate = request.QuotationDate,
            status = request.Status,
            quoteID = request.QuoteId
        });
        await authProviders.Logout();
        return new CreatеRequestResponse { RequestId = requestIdResult.data };
    }

    [Action("Update request", Description = "Update Plunet request")]
    public async Task<BaseResponse> UpdateRequest(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] UpdateRequestRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        var requestClient = Clients.GetRequestClient(authProviders.GetInstanceUrl());
        var response = await requestClient.updateAsync(uuid, new RequestIN
        {
            requestID = request.RequestId,
            briefDescription = request.BriefDescription,
            deliveryDate = request.DeliveryDate,
            orderID = request.OrderId,
            subject = request.Subject,
            quotationDate = request.QuotationDate,
            status = request.Status,
            quoteID = request.QuoteId
        }, true);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Delete request", Description = "Delete a Plunet request")]
    public async Task<BaseResponse> DeleteRequest(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int requestId)
    {
        var uuid = authProviders.GetAuthToken();
        using var requestClient = Clients.GetRequestClient(authProviders.GetInstanceUrl());
        var response = await requestClient.deleteAsync(uuid, requestId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    private RequestResponse MapRequestResponse(Request? request)
    {
        return request == null
            ? new RequestResponse()
            : new RequestResponse
            {
                BriefDescription = request.briefDescription,
                CreationDate = request.creationDate,
                DeliveryDate = request.deliveryDate,
                OrderId = request.orderID,
                QuotationDate = request.quotationDate,
                QuoteId = request.quoteID,
                RequestId = request.requestID,
                Status = request.status,
                Subject = request.subject
            };
    }
}