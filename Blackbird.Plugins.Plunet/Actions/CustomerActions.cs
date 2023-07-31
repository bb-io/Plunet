using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Customer;
using Blackbird.Plugins.Plunet.Utils;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class CustomerActions
{
    [Action("Get customer by name", Description = "Get the Plunet customer by name")]
    public async Task<GetCustomerResponse> GetCustomerByName(
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] [Display("Customer name")]
        string customerName)
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
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] [Display("Customer ID")]
        string customerId)
    {
        var intCustomerId = IntParser.Parse(customerId, nameof(customerId))!.Value;
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
        List<AuthenticationCredentialsProvider> authProviders,
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
    public async Task<BaseResponse> DeleteCustomerById(List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] [Display("Customer ID")]
        string customerId)
    {
        var intCustomerId = IntParser.Parse(customerId, nameof(customerId))!.Value;
        var uuid = authProviders.GetAuthToken();

        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.deleteAsync(uuid, intCustomerId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Create customer", Description = "Create a new customer in Plunet")]
    public async Task<CreateCustomerResponse> CreateCustomer(List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] CreateCustomerRequest request)
    {
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
        
        await authProviders.Logout();
        
        return new CreateCustomerResponse { CustomerId = customerIdResult.data.ToString() };
    }

    [Action("Update customer", Description = "Update Plunet customer")]
    public async Task<BaseResponse> UpdateCustomer(List<AuthenticationCredentialsProvider> authProviders,
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
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] [Display("Customer ID")] string customerId)
    {
        var intCustomerId = IntParser.Parse(customerId, nameof(customerId))!.Value;
        var uuid = authProviders.GetAuthToken();
            
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var paymentInfo = await customerClient.getPaymentInformationAsync(uuid, intCustomerId);
        await authProviders.Logout();

        if (paymentInfo.data is null)
            throw new(paymentInfo.statusMessage);
        
        return new(paymentInfo.data);
    }

    [Action("Set customer address", Description = "Set Plunet cocustomer address")]
    public async Task<SetCustomerAddressResponse> SetCustomerAddress(
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] SetCustomerAddressRequest request)
    {
        var intCustomerId = IntParser.Parse(request.CustomerId, nameof(request.CustomerId))!.Value;
        var uuid = authProviders.GetAuthToken();
        
        var addressClient = Clients.GetCustomerAddressClient(authProviders.GetInstanceUrl());
        var response = await addressClient.insert2Async(uuid, intCustomerId, new()
        {
            name1 = request.FirstAddressName,
            city = request.City,
            addressType = request.AddressType ?? default,
            street = request.Street,
            street2 = request.Street2,
            zip = request.ZipCode,
            country = request.Country,
            state = request.State,
            description = request.Description
        });

        await authProviders.Logout();
        
        return new SetCustomerAddressResponse { AddressId = response.data.ToString() };
    }

    // [Action("Set payment information by customer ID", Description = "Set payment information by Plunet customer ID")]
    // public async Task<BaseResponse> SetPaymentInfoByCustomerId(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId, [ActionParameter] GetPaymentInfoResponse request)
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