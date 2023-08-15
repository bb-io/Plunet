using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Contacts;
using Blackbird.Plugins.Plunet.Models.Customer;
using Blackbird.Plugins.Plunet.Utils;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class ContactActions
{
    [Action("Get customer contacts", Description = "Get all the contacts of the customer")]
    public async Task<GetContactsResponse> GetCustomerContacts(
        IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] CustomerRequest input)
    {
        var intCustomerId = IntParser.Parse(input.CustomerId, nameof(input.CustomerId))!.Value;
        var uuid = authProviders.GetAuthToken();
        
        await using var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var contacts = await dataCustomerContactClient.getAllContactObjectsAsync(uuid, intCustomerId);
        
        await authProviders.Logout();
        return new GetContactsResponse { CustomerContacts = contacts.data.Select(x => new ContactObjectResponse(x)) };
    }

    [Action("Get contact by ID", Description = "Get the Plunet contact by ID")]
    public async Task<ContactObjectResponse> GetContactById(IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] [Display("Contact ID")] string contactId)
    {
        var intContactId = IntParser.Parse(contactId, nameof(contactId))!.Value;
        var uuid = authProviders.GetAuthToken();

        await using var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var contact = await dataCustomerContactClient.getContactObjectAsync(uuid, intContactId);
        await authProviders.Logout();

        if (contact.data is null)
            throw new(contact.statusMessage);
        
        return new(contact.data);
    }

    [Action("Create contact", Description = "Create a new contact in Plunet")]
    public async Task<CreateContactResponse> CreateContact(IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] CreateContactRequest request)
    {
        var intCustomerId = IntParser.Parse(request.CustomerId, nameof(request.CustomerId))!.Value;
        var uuid = authProviders.GetAuthToken();
        
        var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        
        var contactIdResult = await dataCustomerContactClient.insert2Async(uuid, new()
        {
            customerID = intCustomerId,
            name1 = request.FirstName,
            name2 = request.LastName,
            email = request.Email,
            phone = request.Phone,
            mobilePhone = request.MobilePhone,
            costCenter = request.CostCenter,
            externalID = request.ExternalId,
            fax = request.Fax,
            addressID = IntParser.Parse(request.AddressId, nameof(request.AddressId)) ?? default,
            userId = IntParser.Parse(request.UserId, nameof(request.UserId)) ?? default,
            supervisor1 = request.Supervisor1,
            supervisor2 = request.Supervisor2,
            status = IntParser.Parse(request.Status, nameof(request.Status)) ?? default
        });

        await authProviders.Logout();

        return new CreateContactResponse { ContactId = contactIdResult.data.ToString() };
    }

    [Action("Get contact external ID", Description = "Get Plunet contact external ID")]
    public async Task<GetContactExternalIdResponse> GetContactExternalId(
        IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] [Display("Contact ID")] string contactId)
    {
        var intContactId = IntParser.Parse(contactId, nameof(contactId))!.Value;
        var uuid = authProviders.GetAuthToken();
        
        var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var contactExternalId = await dataCustomerContactClient.getExternalIDAsync(uuid, intContactId);
        await authProviders.Logout();
        return new GetContactExternalIdResponse { ContactExternalId = contactExternalId.data };
    }

    [Action("Update contact", Description = "Update Plunet contact")]
    public async Task<BaseResponse> UpdateContact(
        IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] UpdateContactRequest request)
    {
        var intContactId = IntParser.Parse(request.ContactId, nameof(request.ContactId))!.Value;
        var uuid = authProviders.GetAuthToken();
        
        var dataCustomerContactClient = Clients.GetContactClient(authProviders.GetInstanceUrl());
        var response = await dataCustomerContactClient.updateAsync(uuid, new()
        {
            customerContactID = intContactId,
            name1 = request.FirstName,
            name2 = request.LastName,
            email = request.Email,
            phone = request.Phone,
            mobilePhone = request.MobilePhone,
            costCenter = request.CostCenter,
            externalID = request.ExternalId,
            fax = request.Fax,
            addressID = IntParser.Parse(request.AddressId, nameof(request.AddressId)) ?? default,
            customerID = IntParser.Parse(request.CustomerId, nameof(request.CustomerId)) ?? default,
            userId = IntParser.Parse(request.UserId, nameof(request.UserId)) ?? default,
            supervisor1 = request.Supervisor1,
            supervisor2 = request.Supervisor2,
        }, false);
        await authProviders.Logout();
        
        return new BaseResponse { StatusCode = response.statusCode };
    }
}