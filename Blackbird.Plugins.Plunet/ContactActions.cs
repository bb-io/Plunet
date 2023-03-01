using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataCustomerContact30Service;
using Blackbird.Plugins.Plunet.Models.Contacts;

namespace Blackbird.Plugins.Plunet;

[ActionList]
public class ContactActions
{
    [Action]
    public GetContactsResponse GetCustomerContacts(string userName, string password, AuthenticationCredentialsProvider authProvider, [ActionParameter] GetContactRequest request)
    {
        using var dataCustomerContactClient = new DataCustomerContact30Client();
        var contacts = dataCustomerContactClient.getAllContactObjectsAsync(request.UUID, request.CustomerId).GetAwaiter().GetResult();
        return new GetContactsResponse {CustomerContacts = contacts.data.Select(MapContactResponse)};
    }

    private ContactObjectResponse MapContactResponse(CustomerContact customerContact)
    {
        return new ContactObjectResponse
        {
            CostCenter = customerContact.costCenter,
            CustomerContactID = customerContact.customerContactID,
            CustomerID = customerContact.customerID,
            Email = customerContact.email,
            ExternalID = customerContact.externalID,
            Fax = customerContact.fax,
            Name = customerContact.name1??customerContact.name2,
            MobilePhone = customerContact.mobilePhone,
            Phone = customerContact.phone,
            UserId = customerContact.userId,
            Status = customerContact.status,
            Supervisor = customerContact.supervisor1
        };
    }
}