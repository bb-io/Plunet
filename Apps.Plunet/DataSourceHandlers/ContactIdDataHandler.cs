using Apps.Plunet.Api;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Contacts;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.DataSourceHandlers;

public class ContactIdDataHandler : PlunetInvocable, IAsyncDataSourceHandler
{
    public ContactIdDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    // TODO: Make use of advanced dynamic inputs and ask for customer ID first.

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var allStatuses = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var customers = await CustomerClient
            .getAllCustomerObjects2Async(Uuid, Array.ConvertAll(allStatuses, i => (int?)i));

        var allContacts = new List<ContactObjectResponse>();
         
        foreach(var customer in customers.CustomerListResult.data)
        {
            var contacts = await ContactClient.getAllContactObjectsAsync(Uuid, customer.customerID);
               
            if(contacts.data != null)
                allContacts.AddRange(contacts.data.Select(c => new ContactObjectResponse(c)));
        }

        return allContacts.DistinctBy(c => c.CustomerContactId)
            .Where(x => context.SearchString == null ||
                        x.FirstName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase) ||
                        x.LastName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Where(x => !string.IsNullOrWhiteSpace($"{x.FirstName}{x.LastName}"))
            .ToDictionary(x => x.CustomerContactId, x => $"{x.FirstName} {x.LastName}");
    }
}