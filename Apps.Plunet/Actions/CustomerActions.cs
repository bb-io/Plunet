using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Customer;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataCustomer30Service;

namespace Apps.Plunet.Actions;

[ActionList]
public class CustomerActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{
    [Action("Search customers", Description = "Search for specific customers based on specific criteria")]
    public async Task<SearchResponse<GetCustomerResponse>> SearchCustomers([ActionParameter] SearchCustomerRequest input)
    {
        var response = await ExecuteWithRetry<IntegerArrayResult>(async () => await CustomerClient.searchAsync(Uuid, new SearchFilter_Customer
        {
            customerType = ParseId(input.CustomerType),
            email = input.Email ?? string.Empty,
            sourceLanguageCode = input.SourceLanguageCode ?? string.Empty,
            name1 = input.Name1 ?? string.Empty,
            name2 = input.Name2 ?? string.Empty,
            customerStatus = ParseId(input.Status)
        }));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        if (response.data is null)
            return new();

        var results = new List<GetCustomerResponse>();
        foreach (var id in response.data.Where(x => x.HasValue).Take(input.Limit ?? SystemConsts.SearchLimit))
        {
            var customerResponse = await GetCustomerById(new CustomerRequest { CustomerId = id!.Value.ToString() });
            results.Add(customerResponse);
        }

        return new(results);
    }

    [Action("Find customer", Description = "Find a customer based on the specified criteria")]
    public async Task<FindResponse<GetCustomerResponse>?> FindCustomer([ActionParameter] SearchCustomerRequest request)
    {
        var response = await SearchCustomers(request);
        return new(response.Items.FirstOrDefault(), response.TotalCount);
    }

    [Action("Get customer", Description = "Get the Plunet customer")]
    public async Task<GetCustomerResponse> GetCustomerById([ActionParameter] CustomerRequest input)
    {
        var customer = await ExecuteWithRetry<CustomerResult>(async () => await CustomerClient.getCustomerObjectAsync(Uuid, ParseId(input.CustomerId)));
        var paymentInfo = await ExecuteWithRetry<PaymentInfoResult>(async () => await CustomerClient.getPaymentInformationAsync(Uuid, ParseId(input.CustomerId)));

        if (paymentInfo.statusMessage != ApiResponses.Ok)
            throw new(paymentInfo.statusMessage);

        if (customer.data is null)
            throw new(customer.statusMessage);

        var accountManagerResult = await ExecuteWithRetry<IntegerResult>(async () => await CustomerClient.getAccountManagerIDAsync(Uuid, ParseId(input.CustomerId)));
        return new(customer.data, paymentInfo.data, accountManagerResult?.data);
    }

    [Action("Delete customer", Description = "Delete a Plunet customer")]
    public async Task DeleteCustomerById([ActionParameter] CustomerRequest input)
    {
        await ExecuteWithRetry<Result>(async () => await CustomerClient.deleteAsync(Uuid, ParseId(input.CustomerId)));
    }

    [Action("Create customer", Description = "Create a new customer in Plunet")]
    public async Task<GetCustomerResponse> CreateCustomer([ActionParameter] CreateCustomerRequest request)
    {
        if (request.AddressType == null ^ request.Country == null)
            throw new(
                "Both address type and country must be specified to create customer with address or not specified at all");

        var customerIdResult = await ExecuteWithRetry<IntegerResult>(async () => await CustomerClient.insert2Async(Uuid, new()
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
        }));

        var customerId = customerIdResult.data.ToString();

        if (request.AddressType != null)
        {
            await SetCustomerAddress(new()
            {
                CustomerId = customerId
            }, new(request));
        }
        try
        {
            return await GetCustomerById(new CustomerRequest { CustomerId = customerId });
        }
        catch 
        {
            return new GetCustomerResponse { CustomerId = customerId };
        }
    }

    [Action("Update customer", Description = "Update Plunet customer")]
    public async Task<GetCustomerResponse> UpdateCustomer([ActionParameter] CustomerRequest customer,
        [ActionParameter] CreateCustomerRequest request)
    {
        await ExecuteWithRetry<Result>(async () => await CustomerClient.updateAsync(Uuid, new CustomerIN
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
        }, false));

        return await GetCustomerById(customer);
    }

    //[Action("Set customer address", Description = "Set Plunet customer address")]
    public async Task<SetCustomerAddressResponse> SetCustomerAddress(
        [ActionParameter] CustomerRequest customer, [ActionParameter] SetCustomerAddressRequest request)
    {
        var response = await ExecuteWithRetry<DataCustomerAddress30Service.IntegerResult>(async () => await CustomerAddressClient.insert2Async(Uuid, ParseId(customer.CustomerId), new()
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
        }));

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
    
    private async Task<T> ExecuteWithRetry<T>(Func<Task<Result>> func, int maxRetries = 10, int delay = 1000)
        where T : Result
    {
        var attempts = 0;
        while (true)
        {
            Result? result;
            try
            {
                result = await func();
            }
            catch (Exception ex)
            {
                throw new PluginApplicationException($"Error while calling Plunet: {ex.Message}", ex);
            }

            if (result.statusMessage == ApiResponses.Ok)
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

                throw new PluginApplicationException($"No more retries left. Last error: {result.statusMessage}, Session UUID used is invalid.");
            }

            return (T)result;
        }
    }
    
    private async Task<T> ExecuteWithRetry<T>(Func<Task<DataCustomerAddress30Service.Result>> func, int maxRetries = 10, int delay = 1000)
        where T : DataCustomerAddress30Service.Result
    {
        var attempts = 0;
        while (true)
        {
            DataCustomerAddress30Service.Result? result;
            try
            {
                result = await func();
            }
            catch (Exception ex)
            {
                throw new PluginApplicationException($"Error while calling Plunet: {ex.Message}", ex);
            }

            if (result.statusMessage == ApiResponses.Ok)
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

                throw new PluginApplicationException($"No more retries left. Last error: {result.statusMessage}, Session UUID used is invalid.");
            }

            return (T)result;
        }
    }
}