using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models.Customer;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class CustomerActions
{
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
    
    [Action("Get customer by ID", Description = "Get the Plunet customer by ID")]
    public async Task<GetCustomerResponse> GetCustomerById(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter]int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var customer = await customerClient.getCustomerObjectAsync(uuid, customerId);
        await authProviders.Logout();
        return MapCustomerResponse(customer.data);
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