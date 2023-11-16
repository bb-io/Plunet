using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Customer;
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

    [Action("List customers", Description = "List all customers")]
    public async Task<ListCustomersResponse> ListCustomers()
    {
        var uuid = Creds.GetAuthToken();
        var allStatuses = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var response = await CustomerClient
            .getAllCustomerObjects2Async(uuid, Array.ConvertAll(allStatuses, i => (int?)i));
        var customers = response.CustomerListResult.data.Select(x => new GetCustomerResponse(x)).ToArray();
        return new(customers);
    }

    [Action("Does customer exist", Description = "Checks if the customer with the specified name exists")]
    public async Task<bool> DoesCustomerExists([ActionParameter] [Display("Customer name")] string customerName)
    {
        var uuid = Creds.GetAuthToken();
        var response = await CustomerClient.searchAsync(uuid, new SearchFilter_Customer
        {
            name1 = customerName
        });

        return response.data?.Any() is true;
    }

    [Action("Get customer", Description = "Get the Plunet customer")]
    public async Task<GetCustomerResponse> GetCustomerById([ActionParameter] CustomerRequest input)
    {
        var intCustomerId = IntParser.Parse(input.CustomerId, nameof(input.CustomerId))!.Value;
        var uuid = Creds.GetAuthToken();
        var customer = await CustomerClient.getCustomerObjectAsync(uuid, intCustomerId);

        if (customer.data is null)
            throw new(customer.statusMessage);

        return new(customer.data);
    }

    [Action("Delete customer", Description = "Delete a Plunet customer")]
    public async Task DeleteCustomerById([ActionParameter] CustomerRequest input)
    {
        var intCustomerId = IntParser.Parse(input.CustomerId, nameof(input.CustomerId))!.Value;
        var uuid = Creds.GetAuthToken();
        await CustomerClient.deleteAsync(uuid, intCustomerId);
    }

    [Action("Create customer", Description = "Create a new customer in Plunet")]
    public async Task<CreateCustomerResponse> CreateCustomer([ActionParameter] CreateCustomerRequest request)
    {
        if (request.AddressType == null ^ request.Country == null)
            throw new(
                "Both address type and country must be specified to create customer with address or not specified at all");

        var uuid = Creds.GetAuthToken();
        var customerIdResult = await CustomerClient.insert2Async(uuid, new()
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
        var uuid = Creds.GetAuthToken();
        
        await CustomerClient.updateAsync(uuid, new CustomerIN
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
        var uuid = Creds.GetAuthToken();
        var paymentInfo = await CustomerClient.getPaymentInformationAsync(uuid, intCustomerId);

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
        var uuid = Creds.GetAuthToken();
        var addressId = int.Parse(request.AddressId);

        var response = await CustomerAddressClient.updateAsync(uuid, new()
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
        var uuid = Creds.GetAuthToken();
        var intCustomerId = IntParser.Parse(request.CustomerId, nameof(request.CustomerId))!.Value;
        var response = await CustomerAddressClient.getAllAddressesAsync(uuid, intCustomerId);

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