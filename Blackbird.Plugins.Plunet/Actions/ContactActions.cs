using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataCustomerContact30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models;
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
        using var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var contact = await dataCustomerContactClient.getContactObjectAsync(uuid, contactId);
        await authProviders.Logout();
        return MapContactResponse(contact.data);
    }

    //[Display("Contacts")]
    //[Action("Create contact", Description = "Create a new contact in Plunet")]
    //public async Task<CreateContactResponse> CreateContact(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] CreateContactRequest request)
    //{
    //    var uuid = authProviders.GetAuthToken();
    //    var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
    //    var contactIdResult = await dataCustomerContactClient.insert2Async(uuid, new CustomerContactIN
    //    {
    //        name1 = request.FirstName,
    //        name2 = request.LastName,
    //        email = request.Email,
    //        phone = request.Phone,
    //        costCenter = request.CostCenter
    //    });
    //    await authProviders.Logout();
    //    return new CreateContactResponse { ContactId = contactIdResult.data };
    //}

    [Action("Get contact external ID", Description = "Get Plunet contact external ID")]
    public async Task<GetContactExternalIdResponse> GetContactExternalId(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int contactId)
    {
        var uuid = authProviders.GetAuthToken();
        var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var contactExternalId = await dataCustomerContactClient.getExternalIDAsync(uuid, contactId);
        await authProviders.Logout();
        return new GetContactExternalIdResponse { ContactExternalId = contactExternalId.data };
    }

    [Action("Set contact external ID", Description = "Set Plunet contact external ID")]
    public async Task<BaseResponse> SetContactExternalId(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string externalId, [ActionParameter] int contactId)
    {
        var uuid = authProviders.GetAuthToken();
        var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var response = await dataCustomerContactClient.setExternalIDAsync(uuid, externalId, contactId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    //[Display("Contacts")]
    //[Action("Update contact", Description = "Update Plunet contact")]
    //public async Task<BaseResponse> UpdateContact(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] UpdateContactRequest request)
    //{
    //    var uuid = authProviders.GetAuthToken();
    //    var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
    //    var response = await dataCustomerContactClient.updateAsync(uuid, new CustomerContactIN
    //    {
    //        customerContactID = request.ContactId,
    //        name1 = request.FirstName,
    //        name2 = request.LastName,
    //        email = request.Email,
    //        phone = request.Phone,
    //        costCenter = request.CostCenter
    //    }, true);
    //    await authProviders.Logout();
    //    return new BaseResponse { StatusCode = response.statusCode };
    //}

    private ContactObjectResponse MapContactResponse(CustomerContact? customerContact)
    {
        return customerContact == null
            ? new ContactObjectResponse()
            : new ContactObjectResponse
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