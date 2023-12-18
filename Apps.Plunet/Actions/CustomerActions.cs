using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Customer;
using Apps.Plunet.Models.Resource.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
using Blackbird.Plugins.Plunet.DataCustomer30Service;

namespace Apps.Plunet.Actions;

[ActionList]
public class CustomerActions : PlunetInvocable
{
    public CustomerActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Search customers", Description = "Search for specific customers based on specific criteria")]
    public async Task<ListCustomersResponse> SearchCustomers([ActionParameter] SearchCustomerRequest input)
    {
        var response = await CustomerClient.searchAsync(Uuid, new SearchFilter_Customer
        {
            customerType = ParseId(input.CustomerType),
            email = input.Email ?? string.Empty,
            sourceLanguageCode = input.SourceLanguageCode ?? string.Empty,
            name1 = input.Name1 ?? string.Empty,
            name2 = input.Name2 ?? string.Empty,
            customerStatus = ParseId(input.Status)
        });

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        if (response.data is null)
            return new(Enumerable.Empty<GetCustomerResponse>());

        var ids = response.data.Where(x => x.HasValue)
            .Select(x => GetCustomerById(new CustomerRequest { CustomerId = x!.Value.ToString() }))
            .ToArray();

        var result = await Task.WhenAll(ids);

        return new(result);
        
    }

    [Action("Get customer", Description = "Get the Plunet customer")]
    public async Task<GetCustomerResponse> GetCustomerById([ActionParameter] CustomerRequest input)
    {
        var customer = await CustomerClient.getCustomerObjectAsync(Uuid, ParseId(input.CustomerId));

        var paymentInfo = await CustomerClient.getPaymentInformationAsync(Uuid, ParseId(input.CustomerId));

        if (paymentInfo.statusMessage != ApiResponses.Ok)
            throw new(paymentInfo.statusMessage);

        if (customer.data is null)
            throw new(customer.statusMessage);

        var accountManagerId = await CustomerClient.getAccountManagerIDAsync(Uuid, ParseId(input.CustomerId));

        return new(customer.data, paymentInfo.data, accountManagerId?.data);
    }

    [Action("Delete customer", Description = "Delete a Plunet customer")]
    public async Task DeleteCustomerById([ActionParameter] CustomerRequest input)
    {
        await CustomerClient.deleteAsync(Uuid, ParseId(input.CustomerId));
    }

    [Action("Create customer", Description = "Create a new customer in Plunet")]
    public async Task<GetCustomerResponse> CreateCustomer([ActionParameter] CreateCustomerRequest request)
    {
        if (request.AddressType == null ^ request.Country == null)
            throw new(
                "Both address type and country must be specified to create customer with address or not specified at all");

        var customerIdResult = await CustomerClient.insert2Async(Uuid, new()
        {
            name1 = request.Name1,
            name2 = request.Name2,
            website = request.Website,
            formOfAddress = request.FormOfAddress ?? default,
            status = request.Status ?? default,
            email = request.Email,
            mobilePhone = request.MobilePhone,
            costCenter = request.CostCenter,
            academicTitle = request.AcademicTitle,
            currency = request.Currency,
            externalID = request.ExternalId,
            fax = request.Fax,
            fullName = request.FullName,
            opening = request.Opening,
            skypeID = request.SkypeId,
            userId = ParseId(request.UserId),
        });

        var customerId = customerIdResult.data.ToString();

        if (request.AddressType != null)
        {
            await SetCustomerAddress(new()
            {
                CustomerId = customerId
            }, new(request));
        }

        return await GetCustomerById(new CustomerRequest { CustomerId = customerId });
    }

    [Action("Update customer", Description = "Update Plunet customer")]
    public async Task<GetCustomerResponse> UpdateCustomer([ActionParameter] CustomerRequest customer, [ActionParameter] CreateCustomerRequest request)
    {        
        await CustomerClient.updateAsync(Uuid, new CustomerIN
        {
            customerID = ParseId(customer.CustomerId),
            name1 = request.Name1,
            name2 = request.Name2,
            website = request.Website,
            formOfAddress = request.FormOfAddress ?? default,
            status = request.Status ?? default,
            email = request.Email,
            mobilePhone = request.MobilePhone,
            costCenter = request.CostCenter,
            academicTitle = request.AcademicTitle,
            currency = request.Currency,
            externalID = request.ExternalId,
            fax = request.Fax,
            fullName = request.FullName,
            opening = request.Opening,
            skypeID = request.SkypeId,
            userId = ParseId(request.UserId),
        }, false);

        return await GetCustomerById(customer);
    }

    //[Action("Set customer address", Description = "Set Plunet customer address")]
    public async Task<SetCustomerAddressResponse> SetCustomerAddress(
        [ActionParameter] CustomerRequest customer, [ActionParameter] SetCustomerAddressRequest request)
    {        
        var response = await CustomerAddressClient.insert2Async(Uuid, ParseId(customer.CustomerId), new()
        {
            name1 = request.FirstAddressName,
            city = request.City,
            addressType = ParseId(request.AddressType),
            street = request.Street,
            street2 = request.Street2,
            zip = request.ZipCode,
            country = request.Country,
            state = request.State,
            description = request.Description
        });

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        return new()
        {
            AddressId = response.data.ToString()
        };
    }

    //[Action("Update customer address", Description = "Update Plunet customer address")]
    //public async Task<SetCustomerAddressResponse> UpdateCustomerAddress(
    //    [ActionParameter] UpdateCustomerAddressRequest request)
    //{
    //    var addressId = int.Parse(request.AddressId);

    //    var response = await CustomerAddressClient.updateAsync(Uuid, new()
    //    {
    //        addressID = addressId,
    //        name1 = request.FirstAddressName,
    //        city = request.City,
    //        addressType = ParseId(request.AddressType),
    //        street = request.Street,
    //        street2 = request.Street2,
    //        zip = request.ZipCode,
    //        country = request.Country,
    //        state = request.State,
    //        description = request.Description
    //    }, false);

    //    if (response.statusMessage != ApiResponses.Ok)
    //        throw new(response.statusMessage);

    //    return new()
    //    {
    //        AddressId = addressId.ToString()
    //    };
    //}

    //[Action("Get customer addresses", Description = "Get all Plunet customer address IDs")]
    //public async Task<ListAddressesResponse> GetAllAddresses([ActionParameter] CustomerRequest request)
    //{
    //    var response = await CustomerAddressClient.getAllAddressesAsync(Uuid, ParseId(request.CustomerId));

    //    if (response.statusMessage != ApiResponses.Ok)
    //        throw new(response.statusMessage);

    //    var addresses = response.data
    //        .Where(x => x is not null)
    //        .Select(x => x.Value.ToString())
    //        .ToList();

    //    return new(addresses);
    //}

    // [Action("Set payment information by customer ID", Description = "Set payment information by Plunet customer ID")]
    // public async Task SetPaymentInfoByCustomerId(IEnumerable<AuthenticationCredentialsProvider> Creds, [ActionParameter] int customerId, [ActionParameter] GetPaymentInfoResponse request)
    // {
    //     var uuid = Creds.GetAuthToken();
    //     var customerClient = Clients.GetCustomerClient(Creds.GetInstanceUrl());
    //     var response = await customerClient.setPaymentInformationAsync(uuid, customerId, new PaymentInfo
    //     {
    //         accountHolder = request.AccountHolder,
    //         //accountID = request.AccountId,
    //         //BIC = request.BIC,
    //         //contractNumber = request.ContractNumber,
    //         //debitAccount = request.DebitAccount,
    //         IBAN = request.IBAN,
    //         //paymentMethodID = request.PaymentMethodId,
    //         //preselectedTaxID = request.PreselectedTaxId,
    //         //salesTaxID = request.SalesTaxId
    //     });
    //     await Creds.Logout();
    //     return new BaseResponse { StatusCode = response.statusCode };
    // }
}