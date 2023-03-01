using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.Models.Customer;
using Blackbird.Plugins.Plunet.Models.Item;

namespace Blackbird.Plugins.Plunet;

[ActionList]
public class CustomerActions
{
    [Action]
    public GetCustomerResponse GetCustomerByName(string userName, string password, AuthenticationCredentialsProvider authProvider, [ActionParameter] GetCustomerRequest request)
    {
        var customerClient = new DataCustomer30Client();
        var result = customerClient.searchAsync(request.UUID, new SearchFilter_Customer
        {
            name1 = request.CustomerName
        }).GetAwaiter().GetResult().data.FirstOrDefault();
        if (!result.HasValue)
        {
            return MapCustomerResponse(null);
        }
        var customer = customerClient.getCustomerObjectAsync(request.UUID, result.Value).GetAwaiter().GetResult();
        return MapCustomerResponse(customer.data);
    }
    
    [Action]
    public GetCustomerResponse GetCustomerById(string userName, string password, AuthenticationCredentialsProvider authProvider, [ActionParameter] string uuid, [ActionParameter]int customerId)
    {
        var customerClient = new DataCustomer30Client();
        var customer = customerClient.getCustomerObjectAsync(uuid, customerId).GetAwaiter().GetResult();
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