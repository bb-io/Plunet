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
            customerType = IntParser.Parse(input.CustomerType, nameof(input.CustomerType)) ?? -1,
            email = input.Email,
            languageCode = input.LanguageCode,
            sourceLanguageCode = input.SourceLanguageCode,
            name1 = input.Name1,
            name2 = input.Name2,
            customerStatus = IntParser.Parse(input.Status, nameof(input.Status)) ?? -1

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
        var intCustomerId = IntParser.Parse(input.CustomerId, nameof(input.CustomerId))!.Value;
        var customer = await CustomerClient.getCustomerObjectAsync(Uuid, intCustomerId);

        if (customer.data is null)
            throw new(customer.statusMessage);

        return new(customer.data);
    }

    [Action("Delete customer", Description = "Delete a Plunet customer")]
    public async Task DeleteCustomerById([ActionParameter] CustomerRequest input)
    {
        var intCustomerId = IntParser.Parse(input.CustomerId, nameof(input.CustomerId))!.Value;
        await CustomerClient.deleteAsync(Uuid, intCustomerId);
    }

    [Action("Create customer", Description = "Create a new customer in Plunet")]
    public async Task<CreateCustomerResponse> CreateCustomer([ActionParameter] CreateCustomerRequest request)
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
            userId = IntParser.Parse(request.UserId, nameof(request.UserId)) ?? default,
        });

        var customerId = customerIdResult.data.ToString();
        var address = request.AddressType is null
            ? null
            : await SetCustomerAddress(new()
            {
                CustomerId = customerId
            }, new(request));

        return new()
        {
            CustomerId = customerId,
            AddressId = address?.AddressId
        };
    }

    [Action("Update customer", Description = "Update Plunet customer")]
    public async Task UpdateCustomer([ActionParameter] UpdateCustomerRequest request)
    {
        var intCustomerId = IntParser.Parse(request.CustomerId, nameof(request.CustomerId))!.Value;
        
        await CustomerClient.updateAsync(Uuid, new CustomerIN
        {
            customerID = intCustomerId,
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
            userId = IntParser.Parse(request.UserId, nameof(request.UserId)) ?? default,
        }, false);
    }

    [Action("Get payment information", Description = "Get payment information for Plunet customer")]
    public async Task<GetPaymentInfoResponse> GetPaymentInfoByCustomerId([ActionParameter] CustomerRequest input)
    {
        var intCustomerId = IntParser.Parse(input.CustomerId, nameof(input.CustomerId))!.Value;
        var paymentInfo = await CustomerClient.getPaymentInformationAsync(Uuid, intCustomerId);

        if (paymentInfo.data is null)
            throw new(paymentInfo.statusMessage);

        return new(paymentInfo.data);
    }

    [Action("Set customer address", Description = "Set Plunet customer address")]
    public async Task<SetCustomerAddressResponse> SetCustomerAddress(
        [ActionParameter] CustomerRequest customer, [ActionParameter] SetCustomerAddressRequest request)
    {
        var intCustomerId = IntParser.Parse(customer.CustomerId, nameof(customer.CustomerId))!.Value;
        var uuid = Creds.GetAuthToken();
        
        var response = await CustomerAddressClient.insert2Async(uuid, intCustomerId, new()
        {
            name1 = request.FirstAddressName,
            city = request.City,
            addressType = IntParser.Parse(request.AddressType, nameof(request.AddressType)) ?? default,
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

    [Action("Update customer address", Description = "Update Plunet customer address")]
    public async Task<SetCustomerAddressResponse> UpdateCustomerAddress(
        [ActionParameter] UpdateCustomerAddressRequest request)
    {
        var addressId = int.Parse(request.AddressId);

        var response = await CustomerAddressClient.updateAsync(Uuid, new()
        {
            addressID = addressId,
            name1 = request.FirstAddressName,
            city = request.City,
            addressType = IntParser.Parse(request.AddressType, nameof(request.AddressType)) ?? default,
            street = request.Street,
            street2 = request.Street2,
            zip = request.ZipCode,
            country = request.Country,
            state = request.State,
            description = request.Description
        }, false);

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        return new()
        {
            AddressId = addressId.ToString()
        };
    }

    [Action("Get customer addresses", Description = "Get all Plunet customer address IDs")]
    public async Task<ListAddressesResponse> GetAllAddresses([ActionParameter] CustomerRequest request)
    {
        var intCustomerId = IntParser.Parse(request.CustomerId, nameof(request.CustomerId))!.Value;
        var response = await CustomerAddressClient.getAllAddressesAsync(Uuid, intCustomerId);

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        var addresses = response.data
            .Where(x => x is not null)
            .Select(x => x.Value.ToString())
            .ToList();

        return new(addresses);
    }

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