using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Contacts;
using Apps.Plunet.Models.Customer;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;

namespace Apps.Plunet.Actions;

[ActionList]
public class ContactActions : PlunetInvocable
{
    public ContactActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Get customer contacts", Description = "Get all the contacts of the customer")]
    public async Task<GetContactsResponse> GetCustomerContacts([ActionParameter] CustomerRequest input)
    {
        var intCustomerId = IntParser.Parse(input.CustomerId, nameof(input.CustomerId))!.Value;
        var contacts = await ContactClient.getAllContactObjectsAsync(Uuid, intCustomerId);

        return new()
        {
            CustomerContacts = contacts.data.Select(x => new ContactObjectResponse(x))
        };
    }

    [Action("Get contact", Description = "Get the Plunet contact")]
    public async Task<ContactObjectResponse> GetContactById([ActionParameter] ContactRequest request)
    {
        var intContactId = IntParser.Parse(request.ContactId, nameof(request.ContactId))!.Value;
        var contact = await ContactClient.getContactObjectAsync(Uuid, intContactId);
        
        if (contact.data is null)
            throw new(contact.statusMessage);

        return new(contact.data);
    }

    [Action("Get contact by external ID", Description = "Get the Plunet contact by an external ID rather than a Plunet iD")]
    public async Task<ContactObjectResponse> GetContactExternalId([ActionParameter] GetContactByExternalRequest request)
    {
        var response = await ContactClient.seekByExternalIDAsync(Uuid, request.ExternalId);

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        var contactId = response.data.FirstOrDefault();
        if (!contactId.HasValue)
            throw new Exception("No contact found");

        return await GetContactById(new ContactRequest { ContactId = contactId.Value.ToString() });
    }

    [Action("Create contact", Description = "Create a new contact in Plunet")]
    public async Task<ContactObjectResponse> CreateContact([ActionParameter] CreateContactRequest request)
    {
        var intCustomerId = IntParser.Parse(request.CustomerId, nameof(request.CustomerId))!.Value;

        var contactIdResult = await ContactClient.insert2Async(Uuid, new()
        {
            customerID = intCustomerId,
            name1 = request.LastName,
            name2 = request.FirstName,
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

        if (contactIdResult.statusMessage != ApiResponses.Ok)
            throw new(contactIdResult.statusMessage);

        return await GetContactById(new ContactRequest { ContactId = contactIdResult.data.ToString()});
    }

    [Action("Update contact", Description = "Update Plunet contact")]
    public async Task<ContactObjectResponse> UpdateContact([ActionParameter] ContactRequest contact, [ActionParameter] CreateContactRequest request)
    {
        var intContactId = IntParser.Parse(contact.ContactId, nameof(contact.ContactId))!.Value;
        
        var result = await ContactClient.updateAsync(Uuid, new()
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

        if (result.statusMessage != ApiResponses.Ok)
            throw new(result.statusMessage);

        return await GetContactById(contact);
    }
}