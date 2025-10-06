using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Contacts;
using Apps.Plunet.Models.Customer;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataCustomerContact30Service;

namespace Apps.Plunet.Actions;

[ActionList("Contacts")]
public class ContactActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{
    [Action("Get customer contacts", Description = "Get all the contacts of the customer")]
    public async Task<GetContactsResponse> GetCustomerContacts([ActionParameter] CustomerRequest input)
    {
        var contacts = await ExecuteWithRetryAcceptNull(() => ContactClient.getAllContactObjectsAsync(Uuid, ParseId(input.CustomerId)));
        return new()
        {
            CustomerContacts = contacts is null ? new List<ContactObjectResponse>() : contacts.Select(x => new ContactObjectResponse(x))
        };
    }
    
    [Action("Find contact", Description = "Find a contact based on specified parameters")]
    public async Task<ContactObjectResponse?> FindContactByEmail([ActionParameter] CustomerRequest input,
        [ActionParameter] FindContactRequest findContactRequest)
    {
        var contacts =  await ExecuteWithRetryAcceptNull(() => ContactClient.getAllContactObjectsAsync(Uuid, ParseId(input.CustomerId)));
        
        CustomerContact? contact = null;
        if (!string.IsNullOrEmpty(findContactRequest.Email))
        {
            contact = contacts?.FirstOrDefault(x => x.email == findContactRequest.Email);
        }
        
        contact ??= contacts?.FirstOrDefault();

        if (contact is null)
        {
            return null;
        }
        
        return new(contact);
    }

    [Action("Get contact", Description = "Get the Plunet contact")]
    public async Task<ContactObjectResponse> GetContactById([ActionParameter] ContactRequest request)
    {
        var contact = await ExecuteWithRetry(() => ContactClient.getContactObjectAsync(Uuid, ParseId(request.ContactId)));
        return new(contact);
    }

    [Action("Get contact by external ID", Description = "Get the Plunet contact by an external ID rather than a Plunet iD")]
    public async Task<ContactObjectResponse> GetContactExternalId([ActionParameter] GetContactByExternalRequest request)
    {
        var response = await ExecuteWithRetry(() => ContactClient.seekByExternalIDAsync(Uuid, request.ExternalId));
        var contactId = response?.FirstOrDefault();
        return await GetContactById(new ContactRequest { ContactId = contactId?.ToString() });
    }

    [Action("Create contact", Description = "Create a new contact in Plunet")]
    public async Task<ContactObjectResponse> CreateContact([ActionParameter] CreateContactRequest request)
    {
        ContactClient.Endpoint.Binding.SendTimeout = TimeSpan.FromMinutes(5);
        ContactClient.Endpoint.Binding.ReceiveTimeout = TimeSpan.FromMinutes(5);
        var contactIdResult =  await ExecuteWithRetry(() => ContactClient.insert2Async(Uuid, new()
        {
            customerID = ParseId(request.CustomerId),
            name1 = request.LastName,
            name2 = request.FirstName,
            email = request.Email,
            phone = request.Phone,
            mobilePhone = request.MobilePhone,
            costCenter = request.CostCenter,
            externalID = request.ExternalId,
            fax = request.Fax,
            addressID = ParseId(request.AddressId),
            userId = ParseId(request.UserId),
            supervisor1 = request.Supervisor1,
            supervisor2 = request.Supervisor2,
            status = ParseId(request.Status)
        }));

        return await GetContactById(new ContactRequest { ContactId = contactIdResult.ToString()});
    }

    [Action("Update contact", Description = "Update Plunet contact")]
    public async Task<ContactObjectResponse> UpdateContact(
        [ActionParameter] ContactRequest contact, 
        [ActionParameter] CreateContactRequest request)
    {
        var contactToUpdate = await GetContactById(contact);
        
        await ExecuteWithRetry(() => ContactClient.updateAsync(Uuid, new()
        {
            customerContactID = ParseId(contact.ContactId),
            name1 = request.FirstName ?? contactToUpdate.FirstName,
            name2 = request.LastName ?? contactToUpdate.LastName,
            email = request.Email ?? contactToUpdate.Email,
            phone = request.Phone ?? contactToUpdate.Phone,
            mobilePhone = request.MobilePhone ?? contactToUpdate.MobilePhone,
            costCenter = request.CostCenter ?? contactToUpdate.CostCenter,
            externalID = request.ExternalId ?? contactToUpdate.ExternalId,
            fax = request.Fax ?? contactToUpdate.Fax,
            addressID = ParseId(request.AddressId),
            customerID = ParseId(request.CustomerId),
            userId = !string.IsNullOrEmpty(request.UserId) ? ParseId(request.UserId) : ParseId(contactToUpdate.UserId),
            supervisor1 = request.Supervisor1,
            supervisor2 = request.Supervisor2,
            status = !string.IsNullOrEmpty(request.Status) ? ParseId(request.Status) : contactToUpdate.Status
        }, false));

        return await GetContactById(contact);
    }
}