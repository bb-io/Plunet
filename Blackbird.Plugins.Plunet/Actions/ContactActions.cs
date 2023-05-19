using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataCustomerContact30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models.Contacts;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class ContactActions
{
    [Action("Get customer contacts", Description = "Get all the contacts of the customer")]
    public async Task<GetContactsResponse> GetCustomerContacts(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        using var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var contacts = await dataCustomerContactClient.getAllContactObjectsAsync(uuid, customerId);
        await authProviders.Logout();
        return new GetContactsResponse {CustomerContacts = contacts.data.Select(MapContactResponse)};
    }

    [Action("Get contact by ID", Description = "Get the Plunet contact by ID")]
    public async Task<ContactObjectResponse> GetContactById(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int contactId)
    {
        var uuid = authProviders.GetAuthToken();
        var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var contact = await dataCustomerContactClient.getContactObjectAsync(uuid, contactId);
        await authProviders.Logout();
        return MapContactResponse(contact.data);
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