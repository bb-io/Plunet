using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Contacts;
using Blackbird.Plugins.Plunet.Models.Customer;
using DataCustomerAddress30Service;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class CustomerActions
{
    [Display("Customers")]
    [Action("Get customer by name", Description = "Get the Plunet customer by name")]
    public async Task<GetCustomerResponse> GetCustomerByName(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter]string customerName)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var result = (await customerClient.searchAsync(uuid, new SearchFilter_Customer
        {
            name1 = customerName
        })).data.FirstOrDefault();
        if (!result.HasValue)
        {
            await authProviders.Logout();
            return MapCustomerResponse(null);
        }
        var customer = await customerClient.getCustomerObjectAsync(uuid, result.Value);
        await authProviders.Logout();
        return MapCustomerResponse(customer.data);
    }

    [Display("Customers")]
    [Action("Get customer by ID", Description = "Get the Plunet customer by ID")]
    public async Task<GetCustomerResponse> GetCustomerById(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var customer = await customerClient.getCustomerObjectAsync(uuid, customerId);
        await authProviders.Logout();
        return MapCustomerResponse(customer.data);
    }

    [Display("Customers")]
    [Action("Delete customer by name", Description = "Delete the Plunet customer by name")]
    public async Task<BaseResponse> DeleteCustomerByName(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string customerName)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var result = (await customerClient.searchAsync(uuid, new SearchFilter_Customer
        {
            name1 = customerName
        })).data.FirstOrDefault();
        if (!result.HasValue)
        {
            await authProviders.Logout();
            return new BaseResponse();
        }
        var response = await customerClient.deleteAsync(uuid, result.Value);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Display("Customers")]
    [Action("Delete customer by ID", Description = "Delete a Plunet customer by ID")]
    public async Task<BaseResponse> DeleteCustomerById(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.deleteAsync(uuid, customerId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Display("Customers")]
    [Action("Create customer", Description = "Create a new customer in Plunet")]
    public async Task<CreateCustomerResponse> CreateCustomer(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] CreateCustomerRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var customerIdResult = await customerClient.insert2Async(uuid, new CustomerIN
        {
            name1 = request.FirstName,
            name2 = request.LastName,
            website = request.Website,
            //formOfAddress = request.FormOfAddress,
            email = request.Email,
            phone = request.HeadOfficePhone,
            mobilePhone = request.MobilePhone
            //costCenter = request.CostCenter
        });
        await authProviders.Logout();
        return new CreateCustomerResponse { CustomerId = customerIdResult.data };
    }

    [Display("Customers")]
    [Action("Get customer external ID", Description = "Get Plunet customer external ID")]
    public async Task<GetCustomerExternalIdResponse> GetCustomerExternalId(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var customerExternalId = await customerClient.getExternalIDAsync(uuid, customerId);
        await authProviders.Logout();
        return new GetCustomerExternalIdResponse { CustomerExternalId = customerExternalId.data };
    }

    [Display("Customers")]
    [Action("Set customer external ID", Description = "Set Plunet customer external ID")]
    public async Task<BaseResponse> SetCustomerExternalId(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string externalId, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.setExternalIDAsync(uuid, externalId, customerId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Display("Customers")]
    [Action("Update customer", Description = "Update Plunet customer")]
    public async Task<BaseResponse> UpdateCustomer(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] UpdateCustomerRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.updateAsync(uuid, new CustomerIN
        {
            customerID = request.CustomerId,
            name1 = request.FirstName,
            name2 = request.LastName,
            email = request.Email,
            phone = request.Phone,
            costCenter = request.CostCenter
        }, true);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Display("Customers")]
    [Action("Get payment information by customer ID", Description = "Get payment information by Plunet customer ID")]
    public async Task<GetPaymentInfoResponse> GetPaymentInfoByCustomerId(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var paymentInfo = await customerClient.getPaymentInformationAsync(uuid, customerId);
        await authProviders.Logout();
        return MapPaymentInfoResponse(paymentInfo.data);
    }

    [Display("Customers")]
    [Action("Set customer status", Description = "Set Plunet customer status")]
    public async Task<BaseResponse> SetCustomerStatus(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int status, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.setStatusAsync(uuid, status, customerId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Display("Customers")]
    [Action("Get customer status", Description = "Get Plunet customer status")]
    public async Task<GetCustomerStatusResponse> GetCustomerStatus(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var statusResult = await customerClient.getStatusAsync(uuid, customerId);
        await authProviders.Logout();
        return new GetCustomerStatusResponse { Status = statusResult.data };
    }

    [Display("Customers")]
    [Action("Get customer email", Description = "Get Plunet customer email")]
    public async Task<GetCustomerEmailResponse> GetCustomerEmail(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var emailResult = await customerClient.getEmailAsync(uuid, customerId);
        await authProviders.Logout();
        return new GetCustomerEmailResponse { Email = emailResult.data };
    }

    [Display("Customers")]
    [Action("Get customer full name", Description = "Get Plunet customer full name")]
    public async Task<GetCustomerFullNameResponse> GetCustomerFullName(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var fullNameResult = await customerClient.getFullNameAsync(uuid, customerId);
        await authProviders.Logout();
        return new GetCustomerFullNameResponse { FullName = fullNameResult.data };
    }

    [Action("Set customer first name", Description = "Set Plunet customer first name")]
    public async Task<BaseResponse> SetCustomerFirstName(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string firstName, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.setName2Async(uuid, firstName, customerId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Set customer last name", Description = "Set Plunet customer last name")]
    public async Task<BaseResponse> SetCustomerLastName(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string lastName, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.setName1Async(uuid, lastName, customerId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Set customer email", Description = "Set Plunet customer email")]
    public async Task<BaseResponse> SetCustomerEmail(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string email, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.setEmailAsync(uuid, email, customerId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Set customer mobile phone", Description = "Set Plunet customer mobile phone")]
    public async Task<BaseResponse> SetCustomerMobilePhone(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string mobilePhone, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.setMobilePhoneAsync(uuid, mobilePhone, customerId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Set customer telephone number", Description = "Set Plunet customer telephone number")]
    public async Task<BaseResponse> SetCustomerTelephone(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string phoneNumber, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.setPhoneAsync(uuid, phoneNumber, customerId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Set customer website", Description = "Set Plunet customer website")]
    public async Task<BaseResponse> SetContact(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] string website, [ActionParameter] int customerId)
    {
        var uuid = authProviders.GetAuthToken();
        var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
        var response = await customerClient.setWebsiteAsync(uuid, website, customerId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    //[Action("Set customer address", Description = "Set Plunet cocustomer address")]
    //public async Task<CreateContactResponse> SetCustomerAddress(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId)
    //{
    //    var uuid = authProviders.GetAuthToken();
    //    var addressClient = Clients.GetCustomerAddressClient(authProviders.GetInstanceUrl());
    //    var response = await addressClient.insert2Async(uuid, customerId, new AddressIN
    //    {
    //        name1 = "address",
    //        city = "Paris"
    //    });
    //    await authProviders.Logout();
    //    return new CreateContactResponse { ContactId = response.data };
    //}

    //[Action("Set payment information by customer ID", Description = "Set payment information by Plunet customer ID")]
    //public async Task<BaseResponse> SetPaymentInfoByCustomerId(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int customerId, [ActionParameter] GetPaymentInfoResponse request)
    //{
    //    var uuid = authProviders.GetAuthToken();
    //    var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());
    //    var response = await customerClient.setPaymentInformationAsync(uuid, customerId, new PaymentInfo
    //    {
    //        accountHolder = request.AccountHolder,
    //        //accountID = request.AccountId,
    //        //BIC = request.BIC,
    //        //contractNumber = request.ContractNumber,
    //        //debitAccount = request.DebitAccount,
    //        IBAN = request.IBAN,
    //        //paymentMethodID = request.PaymentMethodId,
    //        //preselectedTaxID = request.PreselectedTaxId,
    //        //salesTaxID = request.SalesTaxId
    //    });
    //    await authProviders.Logout();
    //    return new BaseResponse { StatusCode = response.statusCode };
    //}

    private GetCustomerResponse MapCustomerResponse(Customer? customer)
    {
        return customer == null
            ? new GetCustomerResponse()
            : new GetCustomerResponse
            {
                AcademicTitle = customer.academicTitle,
                CostCenter = customer.costCenter,
                Currency = customer.currency,
                CustomerID = customer.customerID,
                Email = customer.email,
                ExternalID = customer.externalID,
                Fax = customer.fax,
                FullName = customer.fullName,
                MobilePhone = customer.mobilePhone,
                Name = customer.name1??customer.name2,
                Phone = customer.phone,
                SkypeID = customer.skypeID,
                Status = customer.status,
                UserId = customer.userId,
                Website = customer.website
            };
    }

    private GetPaymentInfoResponse MapPaymentInfoResponse(PaymentInfo? paymentInfo)
    {
        return paymentInfo == null
            ? new GetPaymentInfoResponse()
            : new GetPaymentInfoResponse
            {
                AccountHolder = paymentInfo.accountHolder,
                AccountId = paymentInfo.accountID,
                BIC = paymentInfo.BIC,
                ContractNumber = paymentInfo.contractNumber,
                DebitAccount = paymentInfo.debitAccount,
                IBAN = paymentInfo.IBAN,
                PaymentMethodId = paymentInfo.paymentMethodID,
                PreselectedTaxId = paymentInfo.preselectedTaxID,
                SalesTaxId = paymentInfo.salesTaxID
            };
    }
}