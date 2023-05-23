using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.DataRequest30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Contacts;
using Blackbird.Plugins.Plunet.Models.Customer;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class CustomerActions
{
    [Display("Customers")]
    [Action("Get customer by name", Description = "Get the Plunet customer by name")]
    public async Task<GetCustomerResponse> GetCustomerByName(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter]string customerName)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var result = (await customerClient.searchAsync(uuid, new SearchFilter_Customer
        {
            name1 = customerName
        })).data.FirstOrDefault();
        if (!result.HasValue)
        {
            await authProviders.Logout();
            return MapCustomerResponse(null);
        }
        var customer = await customerClient.getCustomerObjectAsync(uuid, result.Value);
        await authProviders.Logout();
        return MapCustomerResponse(customer.data);
    }

    [Display("Customers")]
    [Action("Get customer by ID", Description = "Get the Plunet customer by ID")]
    public async Task<GetCustomerResponse> GetCustomerById(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var customer = await customerClient.getCustomerObjectAsync(uuid, customerId);
        await authProviders.Logout();
        return MapCustomerResponse(customer.data);
    }

    [Display("Customers")]
    [Action("Delete customer by name", Description = "Delete the Plunet customer by name")]
    public async Task<BaseResponse> DeleteCustomerByName(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string customerName)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var result = (await customerClient.searchAsync(uuid, new SearchFilter_Customer
        {
            name1 = customerName
        })).data.FirstOrDefault();
        if (!result.HasValue)
        {
            await authProviders.Logout();
            return new BaseResponse();
        }
        var response = await customerClient.deleteAsync(uuid, result.Value);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Display("Customers")]
    [Action("Delete customer by ID", Description = "Delete a Plunet customer by ID")]
    public async Task<BaseResponse> DeleteCustomerById(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.deleteAsync(uuid, customerId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Display("Customers")]
    [Action("Create customer", Description = "Create a new customer in Plunet")]
    public async Task<CreateCustomerResponse> CreateCustomer(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] CreateCustomerRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var customerIdResult = await customerClient.insert2Async(uuid, new CustomerIN
        {
            name1 = request.FirstName,
            name2 = request.LastName,
            email = request.Email,
            phone = request.Phone,
            costCenter = request.CostCenter
        });
        await authProviders.Logout();
        return new CreateCustomerResponse { CustomerId = customerIdResult.data };
    }

    [Display("Customers")]
    [Action("Get customer external ID", Description = "Get Plunet customer external ID")]
    public async Task<GetCustomerExternalIdResponse> GetCustomerExternalId(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var customerExternalId = await customerClient.getExternalIDAsync(uuid, customerId);
        await authProviders.Logout();
        return new GetCustomerExternalIdResponse { CustomerExternalId = customerExternalId.data };
    }

    [Display("Customers")]
    [Action("Set customer external ID", Description = "Set Plunet customer external ID")]
    public async Task<BaseResponse> SetCustomerExternalId(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string externalId, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.setExternalIDAsync(uuid, externalId, customerId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Display("Customers")]
    [Action("Update customer", Description = "Update Plunet customer")]
    public async Task<BaseResponse> UpdateCustomer(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] UpdateCustomerRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.updateAsync(uuid, new CustomerIN
        {
            customerID = request.CustomerId,
            name1 = request.FirstName,
            name2 = request.LastName,
            email = request.Email,
            phone = request.Phone,
            costCenter = request.CostCenter
        }, true);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    private GetCustomerResponse MapCustomerResponse(Customer? customer)
    {
        return customer == null
            ? new GetCustomerResponse()
            : new GetCustomerResponse
            {
                AcademicTitle = customer.academicTitle,
                CostCenter = customer.costCenter,
                Currency = customer.currency,
                CustomerID = customer.customerID,
                Email = customer.email,
                ExternalID = customer.externalID,
                Fax = customer.fax,
                FullName = customer.fullName,
                MobilePhone = customer.mobilePhone,
                Name = customer.name1??customer.name2,
                Phone = customer.phone,
                SkypeID = customer.skypeID,
                Status = customer.status,
                UserId = customer.userId,
                Website = customer.website
            };
    }
}