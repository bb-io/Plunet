using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataCustomerContact30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Contacts;
using DataResourceAddress30Service;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class ContactActions
{
    [Display("Contacts")]
    [Action("Get customer contacts", Description = "Get all the contacts of the customer")]
    public async Task<GetContactsResponse> GetCustomerContacts(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        using var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var contacts = await dataCustomerContactClient.getAllContactObjectsAsync(uuid, customerId);
        await authProviders.Logout();
        return new GetContactsResponse {CustomerContacts = contacts.data.Select(MapContactResponse)};
    }

    [Display("Contacts")]
    [Action("Get contact by ID", Description = "Get the Plunet contact by ID")]
    public async Task<ContactObjectResponse> GetContactById(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int contactId)
    {
        var uuid = authProviders.GetAuthToken();
        using var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var contact = await dataCustomerContactClient.getContactObjectAsync(uuid, contactId);
        await authProviders.Logout();
        return MapContactResponse(contact.data);
    }

    [Action("Create contact", Description = "Create a new contact in Plunet")]
    public async Task<CreateContactResponse> CreateContact(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] CreateContactRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        //var contactIdResult = await dataCustomerContactClient.insert2Async(uuid, new CustomerContactIN
        //{
        //    customerID = request.CustomerId,
        //    name1 = request.LastName,
        //    name2 = request.FirstName,
        //    email = request.Email
        //});
        var contactIdResult = await dataCustomerContactClient.insertAsync(uuid, request.CustomerId);
        dataCustomerContactClient.setName2Async(uuid, request.FirstName, contactIdResult.data);
        dataCustomerContactClient.setName1Async(uuid, request.LastName, contactIdResult.data);
        dataCustomerContactClient.setEmailAsync(uuid, request.Email, contactIdResult.data);
        dataCustomerContactClient.setEmailAsync(uuid, request.Email, contactIdResult.data);
        dataCustomerContactClient.setMobilePhoneAsync(uuid, request.MobilePhone, contactIdResult.data);

        await authProviders.Logout();

        return new CreateContactResponse { ContactId = contactIdResult.data };
    }

    [Display("Contacts")]
    [Action("Get contact external ID", Description = "Get Plunet contact external ID")]
    public async Task<GetContactExternalIdResponse> GetContactExternalId(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int contactId)
    {
        var uuid = authProviders.GetAuthToken();
        var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var contactExternalId = await dataCustomerContactClient.getExternalIDAsync(uuid, contactId);
        await authProviders.Logout();
        return new GetContactExternalIdResponse { ContactExternalId = contactExternalId.data };
    }

    [Display("Contacts")]
    [Action("Set contact external ID", Description = "Set Plunet contact external ID")]
    public async Task<BaseResponse> SetContactExternalId(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string externalId, [ActionParameter] int contactId)
    {
        var uuid = authProviders.GetAuthToken();
        var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var response = await dataCustomerContactClient.setExternalIDAsync(uuid, externalId, contactId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Set contact first name", Description = "Set Plunet contact first name")]
    public async Task<BaseResponse> SetContactFirstName(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string firstName, [ActionParameter] int contactId)
    {
        var uuid = authProviders.GetAuthToken();
        var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var response = await dataCustomerContactClient.setName2Async(uuid, firstName, contactId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Set contact last name", Description = "Set Plunet contact last name")]
    public async Task<BaseResponse> SetContactLastName(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string lastName, [ActionParameter] int contactId)
    {
        var uuid = authProviders.GetAuthToken();
        var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var response = await dataCustomerContactClient.setName1Async(uuid, lastName, contactId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Set contact email", Description = "Set Plunet contact email")]
    public async Task<BaseResponse> SetContactEmail(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string email, [ActionParameter] int contactId)
    {
        var uuid = authProviders.GetAuthToken();
        var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var response = await dataCustomerContactClient.setEmailAsync(uuid, email, contactId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Set contact mobile phone", Description = "Set Plunet contact mobile phone")]
    public async Task<BaseResponse> SetContactMobilePhone(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string mobilePhone, [ActionParameter] int contactId)
    {
        var uuid = authProviders.GetAuthToken();
        var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var response = await dataCustomerContactClient.setMobilePhoneAsync(uuid, mobilePhone, contactId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Set contact telephone number", Description = "Set Plunet contact telephone number")]
    public async Task<BaseResponse> SetContactTelephone(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string phoneNumber, [ActionParameter] int contactId)
    {
        var uuid = authProviders.GetAuthToken();
        var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var response = await dataCustomerContactClient.setPhoneAsync(uuid, phoneNumber, contactId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    //[Display("Address ID")]
    //[Action("Set contact address", Description = "Set Plunet contact address")]
    //public async Task<int> SetContactAddress(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int resourceId)
    //{
    //    var uuid = authProviders.GetAuthToken();
    //    var addressClient = Clients.GetResourceAddressClient(authProviders.GetInstanceUrl());
    //    var response = await addressClient.insert2Async(uuid, resourceId, new AddressIN 
    //    {
    //        name1 = "address",
    //        city = "Paris",
    //        addressType = 1,

    //    });
    //    await authProviders.Logout();
    //    return  response.data;
    //}

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