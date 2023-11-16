﻿using Apps.Plunet.Api;
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
        var uuid = Creds.GetAuthToken();
        var contacts = await ContactClient.getAllContactObjectsAsync(uuid, intCustomerId);

        return new()
        {
            CustomerContacts = contacts.data.Select(x => new ContactObjectResponse(x))
        };
    }

    [Action("List contacts", Description = "List contacts")]
    public async Task<GetContactsResponse> GetCustomerContacts2()
    {
        var uuid = Creds.GetAuthToken();
        var allStatuses = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var customers = await CustomerClient
            .getAllCustomerObjects2Async(uuid, Array.ConvertAll(allStatuses, i => (int?)i));

        var allContacts = new List<ContactObjectResponse>();
        foreach (var customer in customers.CustomerListResult.data)
        {
            var contacts = await ContactClient.getAllContactObjectsAsync(uuid, customer.customerID);
            if (contacts.data != null)
            {
                allContacts.AddRange(contacts.data.Select(c => new ContactObjectResponse(c)));
            }
        }
        
        return new()
        {
            CustomerContacts = allContacts.DistinctBy(x => x.CustomerContactId)
        };
    }

    [Action("Get contact", Description = "Get the Plunet contact")]
    public async Task<ContactObjectResponse> GetContactById([ActionParameter] ContactRequest request)
    {
        var intContactId = IntParser.Parse(request.ContactId, nameof(request.ContactId))!.Value;
        var uuid = Creds.GetAuthToken();
        var contact = await ContactClient.getContactObjectAsync(uuid, intContactId);
        
        if (contact.data is null)
            throw new(contact.statusMessage);

        return new(contact.data);
    }

    [Action("Create contact", Description = "Create a new contact in Plunet")]
    public async Task<CreateContactResponse> CreateContact([ActionParameter] CreateContactRequest request)
    {
        var intCustomerId = IntParser.Parse(request.CustomerId, nameof(request.CustomerId))!.Value;
        var uuid = Creds.GetAuthToken();

        var contactIdResult = await ContactClient.insert2Async(uuid, new()
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
        
        return new()
        {
            ContactId = contactIdResult.data.ToString()
        };
    }

    [Action("Get contact external ID", Description = "Get Plunet contact external ID")]
    public async Task<GetContactExternalIdResponse> GetContactExternalId([ActionParameter] ContactRequest request)
    {
        var intContactId = IntParser.Parse(request.ContactId, nameof(request.ContactId))!.Value;
        var uuid = Creds.GetAuthToken();
        var contactExternalId = await ContactClient.getExternalIDAsync(uuid, intContactId);

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
        
        await ContactClient.updateAsync(uuid, new()
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
    }
}