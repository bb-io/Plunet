using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Contacts;
using Apps.Plunet.Models.Customer;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataCustomerContact30Service;

namespace Apps.Plunet.Actions;

[ActionList]
public class ContactActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{
    [Action("Get customer contacts", Description = "Get all the contacts of the customer")]
    public async Task<GetContactsResponse> GetCustomerContacts([ActionParameter] CustomerRequest input)
    {
        var contacts = await ExecuteWithRetry<CustomerContactListResult>(async () =>  await ContactClient.getAllContactObjectsAsync(Uuid, ParseId(input.CustomerId)));
        return new()
        {
            CustomerContacts = contacts.data is null ? new List<ContactObjectResponse>() : contacts.data.Select(x => new ContactObjectResponse(x))
        };
    }

    [Action("Get customer emails", Description = "Get all the emails of the specified customer")]
    public async Task<GetContactsEmailResponse> GetCustomerEmails([ActionParameter] CustomerRequest input)
    {
        var contacts =  await ExecuteWithRetry<CustomerContactListResult>(async () =>  await ContactClient.getAllContactObjectsAsync(Uuid, ParseId(input.CustomerId)));
        return new GetContactsEmailResponse
        {
            EmailAddresses = contacts.data is null ? new List<string>() : contacts.data.Select(x => x.email)
        };
    }

    [Action("Get contact", Description = "Get the Plunet contact")]
    public async Task<ContactObjectResponse> GetContactById([ActionParameter] ContactRequest request)
    {
        var contact = await ExecuteWithRetry<CustomerContactResult>(async () =>
            await ContactClient.getContactObjectAsync(Uuid, ParseId(request.ContactId)));
        
        if (contact.data is null)
            throw new(contact.statusMessage);

        return new(contact.data);
    }

    [Action("Get contact by external ID", Description = "Get the Plunet contact by an external ID rather than a Plunet iD")]
    public async Task<ContactObjectResponse> GetContactExternalId([ActionParameter] GetContactByExternalRequest request)
    {
        var response = await ExecuteWithRetry<IntegerArrayResult>(async () =>  await ContactClient.seekByExternalIDAsync(Uuid, request.ExternalId));

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
        var contactIdResult =  await ExecuteWithRetry<IntegerResult>(async () => await ContactClient.insert2Async(Uuid, new()
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

        if (contactIdResult.statusMessage != ApiResponses.Ok)
            throw new(contactIdResult.statusMessage);

        return await GetContactById(new ContactRequest { ContactId = contactIdResult.data.ToString()});
    }

    [Action("Update contact", Description = "Update Plunet contact")]
    public async Task<ContactObjectResponse> UpdateContact([ActionParameter] ContactRequest contact, [ActionParameter] CreateContactRequest request)
    {        
        var result =  await ExecuteWithRetry<Result>(async () => await ContactClient.updateAsync(Uuid, new()
        {
            customerContactID = ParseId(contact.ContactId),
            name1 = request.FirstName,
            name2 = request.LastName,
            email = request.Email,
            phone = request.Phone,
            mobilePhone = request.MobilePhone,
            costCenter = request.CostCenter,
            externalID = request.ExternalId,
            fax = request.Fax,
            addressID = ParseId(request.AddressId),
            customerID = ParseId(request.CustomerId),
            userId = ParseId(request.UserId),
            supervisor1 = request.Supervisor1,
            supervisor2 = request.Supervisor2,
        }, false));

        if (result.statusMessage != ApiResponses.Ok)
            throw new(result.statusMessage);

        return await GetContactById(contact);
    }
    
    private async Task<T> ExecuteWithRetry<T>(Func<Task<Result>> func, int maxRetries = 10, int delay = 1000)
        where T : Result
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();
            
            if(result.statusMessage == ApiResponses.Ok)
            {
                return (T)result;
            }
            
            if(result.statusMessage.Contains("session-UUID used is invalid"))
            {
                if (attempts < maxRetries)
                {
                    await Task.Delay(delay);
                    await RefreshAuthToken();
                    attempts++;
                    continue;
                }

                throw new($"No more retries left. Last error: {result.statusMessage}, Session UUID used is invalid.");
            }

            return (T)result;
        }
    }
}