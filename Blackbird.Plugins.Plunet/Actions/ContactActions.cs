using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
using Blackbird.Plugins.Plunet.Api;
using Blackbird.Plugins.Plunet.DataSourceHandlers;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Invocables;
using Blackbird.Plugins.Plunet.Models.Contacts;
using Blackbird.Plugins.Plunet.Models.Customer;

namespace Blackbird.Plugins.Plunet.Actions;

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
        var uuid = Creds.GetAuthToken();

        await using var dataCustomerContactClient = Clients.GetContactClient(Creds.GetInstanceUrl());
        var contacts = await dataCustomerContactClient.getAllContactObjectsAsync(uuid, intCustomerId);

        await Creds.Logout();

        return new()
        {
            CustomerContacts = contacts.data.Select(x => new ContactObjectResponse(x))
        };
    }

    [Action("List contacts", Description = "List contacts")]
    public async Task<GetContactsResponse> GetCustomerContacts2()
    {
        var uuid = Creds.GetAuthToken();
        await using var customerClient = Clients.GetCustomerClient(Creds.GetInstanceUrl());
        await using var contactClient = Clients.GetContactClient(Creds.GetInstanceUrl());

        var allStatuses = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var customers = await customerClient
            .getAllCustomerObjects2Async(uuid, Array.ConvertAll(allStatuses, i => (int?)i));

        var allContacts = new List<ContactObjectResponse>();
        foreach (var customer in customers.CustomerListResult.data)
        {
            var contacts = await contactClient.getAllContactObjectsAsync(uuid, customer.customerID);
            if (contacts.data != null)
            {
                allContacts.AddRange(contacts.data.Select(c => new ContactObjectResponse(c)));
            }
        }

        await Creds.Logout();
        return new()
        {
            CustomerContacts = allContacts.DistinctBy(x => x.CustomerContactId)
        };
    }

    [Action("Get contact by ID", Description = "Get the Plunet contact by ID")]
    public async Task<ContactObjectResponse> GetContactById(
        [ActionParameter] [Display("Contact ID")] [DataSource(typeof(ContactIdDataHandler))]
        string contactId)
    {
        var intContactId = IntParser.Parse(contactId, nameof(contactId))!.Value;
        var uuid = Creds.GetAuthToken();

        await using var dataCustomerContactClient = Clients.GetContactClient(Creds.GetInstanceUrl());
        var contact = await dataCustomerContactClient.getContactObjectAsync(uuid, intContactId);
        await Creds.Logout();

        if (contact.data is null)
            throw new(contact.statusMessage);

        return new(contact.data);
    }

    [Action("Create contact", Description = "Create a new contact in Plunet")]
    public async Task<CreateContactResponse> CreateContact([ActionParameter] CreateContactRequest request)
    {
        var intCustomerId = IntParser.Parse(request.CustomerId, nameof(request.CustomerId))!.Value;
        var uuid = Creds.GetAuthToken();

        var dataCustomerContactClient = Clients.GetContactClient(Creds.GetInstanceUrl());

        var contactIdResult = await dataCustomerContactClient.insert2Async(uuid, new()
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

        await Creds.Logout();

        return new()
        {
            ContactId = contactIdResult.data.ToString()
        };
    }

    [Action("Get contact external ID", Description = "Get Plunet contact external ID")]
    public async Task<GetContactExternalIdResponse> GetContactExternalId(
        [ActionParameter] [Display("Contact ID")]
        string contactId)
    {
        var intContactId = IntParser.Parse(contactId, nameof(contactId))!.Value;
        var uuid = Creds.GetAuthToken();

        var dataCustomerContactClient = Clients.GetContactClient(Creds.GetInstanceUrl());
        var contactExternalId = await dataCustomerContactClient.getExternalIDAsync(uuid, intContactId);

        await Creds.Logout();

        return new()
        {
            ContactExternalId = contactExternalId.data
        };
    }

    [Action("Update contact", Description = "Update Plunet contact")]
    public async Task UpdateContact([ActionParameter] UpdateContactRequest request)
    {
        var intContactId = IntParser.Parse(request.ContactId, nameof(request.ContactId))!.Value;
        var uuid = Creds.GetAuthToken();

        var dataCustomerContactClient = Clients.GetContactClient(Creds.GetInstanceUrl());
        await dataCustomerContactClient.updateAsync(uuid, new()
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

        await Creds.Logout();
    }
}