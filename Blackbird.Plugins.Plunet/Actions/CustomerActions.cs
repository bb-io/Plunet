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
    [Action]
    public async Task<GetCustomerResponse> GetCustomerByName(IEnumerable<AuthenticationCredentialsProvider> authProviders, [ActionParameter] GetCustomerRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = new DataCustomer30Client();
        var result = (await customerClient.searchAsync(uuid, new SearchFilter_Customer
        {
            name1 = request.CustomerName
        })).data.FirstOrDefault();
        if (!result.HasValue)
        {
            return MapCustomerResponse(null);
        }
        var customer = await customerClient.getCustomerObjectAsync(uuid, result.Value);
        return MapCustomerResponse(customer.data);
    }
    
    [Action]
    public async Task<GetCustomerResponse> GetCustomerById(IEnumerable<AuthenticationCredentialsProvider> authProviders, [ActionParameter]int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = new DataCustomer30Client();
        var customer = await customerClient.getCustomerObjectAsync(uuid, customerId);
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