using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.Constants;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Customer;
using Blackbird.Plugins.Plunet.Utils;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class CustomerActions
{
    [Action("List customers", Description = "List all customers")]
    public async Task<ListCustomersResponse> ListCustomers(
        IEnumerable<AuthenticationCredentialsProvider> authProviders)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());

        var allStatuses = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var resposne = await customerClient
            .getAllCustomerObjects2Async(uuid, Array.ConvertAll(allStatuses, i => (int?)i));

        await authProviders.Logout();

        var customers = resposne.CustomerListResult.data
            .Select(x => new GetCustomerResponse(x)).ToArray();
        return new(customers);
    }

    [Action("Does customer exist", Description = "Checks if the customer with the specified name exists")]
    public async Task<bool> DoesCustomerExists(
        IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] [Display("Customer name")]
        string customerName)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.searchAsync(uuid, new SearchFilter_Customer
        {
            name1 = customerName
        });

        return response.data?.Any() is true;
    }

    [Action("Get customer by name", Description = "Get the Plunet customer by name")]
    public async Task<GetCustomerResponse> GetCustomerByName(
        IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] [Display("Customer name")]
        string customerName)
    {
        try
        {
            var uuid = authProviders.GetAuthToken();
            var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
            var response = await customerClient.searchAsync(uuid, new SearchFilter_Customer
            {
                name1 = customerName
            });

            var result = response.data?.FirstOrDefault();

            if (!result.HasValue)
                throw new("No customer found");

            var customer = await customerClient.getCustomerObjectAsync(uuid, result.Value);

            if (customer.data is null)
                throw new(customer.statusMessage);

            return new(customer.data);
        }
        finally
        {
            await authProviders.Logout();
        }
    }

    [Action("Get customer by ID", Description = "Get the Plunet customer by ID")]
    public async Task<GetCustomerResponse> GetCustomerById(
        IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] CustomerRequest input)
    {
        var intCustomerId = IntParser.Parse(input.CustomerId, nameof(input.CustomerId))!.Value;
        var uuid = authProviders.GetAuthToken();

        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var customer = await customerClient.getCustomerObjectAsync(uuid, intCustomerId);
        await authProviders.Logout();

        if (customer.data is null)
            throw new(customer.statusMessage);

        return new(customer.data);
    }

    [Action("Delete customer by name", Description = "Delete the Plunet customer by name")]
    public async Task<BaseResponse> DeleteCustomerByName(
        IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] string customerName)
    {
        try
        {
            var uuid = authProviders.GetAuthToken();
            var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
            var result = (await customerClient.searchAsync(uuid, new SearchFilter_Customer
            {
                name1 = customerName
            })).data.FirstOrDefault();

            if (!result.HasValue)
                throw new("No user found");

            var response = await customerClient.deleteAsync(uuid, result.Value);
            return new BaseResponse { StatusCode = response.statusCode };
        }
        finally
        {
            await authProviders.Logout();
        }
    }

    [Action("Delete customer by ID", Description = "Delete a Plunet customer by ID")]
    public async Task<BaseResponse> DeleteCustomerById(IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] CustomerRequest input)
    {
        var intCustomerId = IntParser.Parse(input.CustomerId, nameof(input.CustomerId))!.Value;
        var uuid = authProviders.GetAuthToken();

        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.deleteAsync(uuid, intCustomerId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Create customer", Description = "Create a new customer in Plunet")]
    public async Task<CreateCustomerResponse> CreateCustomer(
        IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] CreateCustomerRequest request)
    {
        if (request.AddressType == null ^ request.Country == null)
            throw new(
                "Both address type and country must be specified to create customer with address or not specified at all");

        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var customerIdResult = await customerClient.insert2Async(uuid, new()
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
            : await SetCustomerAddress(authProviders, new()
            {
                CustomerId = customerId
            }, new(request));

        await authProviders.Logout();

        return new CreateCustomerResponse
        {
            CustomerId = customerId,
            AddressId = address?.AddressId
        };
    }

    [Action("Update customer", Description = "Update Plunet customer")]
    public async Task<BaseResponse> UpdateCustomer(IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] UpdateCustomerRequest request)
    {
        var intCustomerId = IntParser.Parse(request.CustomerId, nameof(request.CustomerId))!.Value;
        var uuid = authProviders.GetAuthToken();

        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.updateAsync(uuid, new CustomerIN
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

        await authProviders.Logout();

        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Get payment information by customer ID", Description = "Get payment information by Plunet customer ID")]
    public async Task<GetPaymentInfoResponse> GetPaymentInfoByCustomerId(
        IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] CustomerRequest input)
    {
        var intCustomerId = IntParser.Parse(input.CustomerId, nameof(input.CustomerId))!.Value;
        var uuid = authProviders.GetAuthToken();

        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var paymentInfo = await customerClient.getPaymentInformationAsync(uuid, intCustomerId);
        await authProviders.Logout();

        if (paymentInfo.data is null)
            throw new(paymentInfo.statusMessage);

        return new(paymentInfo.data);
    }

    [Action("Set customer address", Description = "Set Plunet customer address")]
    public async Task<SetCustomerAddressResponse> SetCustomerAddress(
        IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] CustomerRequest customer,
        [ActionParameter] SetCustomerAddressRequest request)
    {
        var intCustomerId = IntParser.Parse(customer.CustomerId, nameof(customer.CustomerId))!.Value;
        var uuid = authProviders.GetAuthToken();

        var addressClient = Clients.GetCustomerAddressClient(authProviders.GetInstanceUrl());
        var response = await addressClient.insert2Async(uuid, intCustomerId, new()
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

        await authProviders.Logout();

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        return new SetCustomerAddressResponse { AddressId = response.data.ToString() };
    }

    [Action("Update customer address", Description = "Update Plunet customer address")]
    public async Task<SetCustomerAddressResponse> UpdateCustomerAddress(
        IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] UpdateCustomerAddressRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        var addressId = int.Parse(request.AddressId);

        var addressClient = Clients.GetCustomerAddressClient(authProviders.GetInstanceUrl());

        var response = await addressClient.updateAsync(uuid, new()
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

        await authProviders.Logout();

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        return new SetCustomerAddressResponse { AddressId = addressId.ToString() };
    }

    [Action("Get customer addresses", Description = "Get all Plunet customer address IDs")]
    public async Task<ListAddressesResponse> GetAllAddresses(
        IEnumerable<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] CustomerRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        var intCustomerId = IntParser.Parse(request.CustomerId, nameof(request.CustomerId))!.Value;

        var addressClient = Clients.GetCustomerAddressClient(authProviders.GetInstanceUrl());
        var response = await addressClient.getAllAddressesAsync(uuid, intCustomerId);

        await authProviders.Logout();

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        var addresses = response.data
            .Where(x => x is not null)
            .Select(x => x.Value)
            .ToList();
        
        return new(addresses);
    }

    // [Action("Set payment information by customer ID", Description = "Set payment information by Plunet customer ID")]
    // public async Task<BaseResponse> SetPaymentInfoByCustomerId(IEnumerable<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId, [ActionParameter] GetPaymentInfoResponse request)
    // {
    //     var uuid = authProviders.GetAuthToken();
    //     var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
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
    //     await authProviders.Logout();
    //     return new BaseResponse { StatusCode = response.statusCode };
    // }
}