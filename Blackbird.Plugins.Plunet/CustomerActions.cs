using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.Models.Customer;

namespace Blackbird.Plugins.Plunet;

[ActionList]
public class CustomerActions
{
    [Action]
    public GetCustomerResponse GetCustomerByName(string url, string username, string password, AuthenticationCredentialsProvider authProvider, [ActionParameter]string customerName)
    {
        using var authClient = Clients.GetAuthClient(url);
        var uuid = authClient.loginAsync(username, password).GetAwaiter().GetResult();
        using var customerClient = Clients.GetCustomerClient(url);
        var result = customerClient.searchAsync(uuid, new SearchFilter_Customer
        {
            name1 = customerName
        }).GetAwaiter().GetResult().data.FirstOrDefault();
        if (!result.HasValue)
        {
            return MapCustomerResponse(null);
        }
        var customer = customerClient.getCustomerObjectAsync(uuid, result.Value).GetAwaiter().GetResult();
        authClient.logoutAsync(uuid).GetAwaiter().GetResult();
        return MapCustomerResponse(customer.data);
    }
    
    [Action]
    public GetCustomerResponse GetCustomerById(string url, string username, string password, AuthenticationCredentialsProvider authProvider, [ActionParameter]int customerId)
    {
        using var authClient = Clients.GetAuthClient(url);
        var uuid = authClient.loginAsync(username, password).GetAwaiter().GetResult();
        using var customerClient = Clients.GetCustomerClient(url);
        var customer = customerClient.getCustomerObjectAsync(uuid, customerId).GetAwaiter().GetResult();
        authClient.logoutAsync(uuid).GetAwaiter().GetResult();
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