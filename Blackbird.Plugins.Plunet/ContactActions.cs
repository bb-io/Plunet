﻿using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataCustomerContact30Service;
using Blackbird.Plugins.Plunet.Models.Contacts;
using System.Security.Policy;

namespace Blackbird.Plugins.Plunet;

[ActionList]
public class ContactActions
{
    [Action]
    public GetContactsResponse GetCustomerContacts(string url, string username, string password, AuthenticationCredentialsProvider authProvider, [ActionParameter]int customerId)
    {
        using var authClient = Clients.GetAuthClient(url);
        var uuid = authClient.loginAsync(username, password).GetAwaiter().GetResult();
        using var dataCustomerContactClient = Clients.GetContactClient(url);
        var contacts = dataCustomerContactClient.getAllContactObjectsAsync(uuid, customerId).GetAwaiter().GetResult();
        authClient.logoutAsync(uuid).GetAwaiter().GetResult();
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