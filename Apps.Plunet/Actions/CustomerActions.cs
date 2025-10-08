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

[ActionList("Customers")]
public class CustomerActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{
    [Action("Search customers", Description = "Search for specific customers based on specific criteria")]
    public async Task<SearchResponse<GetCustomerResponse>> SearchCustomers([ActionParameter] SearchCustomerRequest input)
    {
        var response = await ExecuteWithRetryAcceptNull(() => CustomerClient.searchAsync(Uuid, new SearchFilter_Customer
        {
            customerType = ParseId(input.CustomerType),
            email = input.Email ?? string.Empty,
            sourceLanguageCode = input.SourceLanguageCode ?? string.Empty,
            name1 = input.Name1 ?? string.Empty,
            name2 = input.Name2 ?? string.Empty,
            customerStatus = ParseId(input.Status)
        }));

        if (response is null)
            return new();

        var limitedIds = response
        .Where(x => x.HasValue)
        .Select(x => x!.Value)
        .Take(input.Limit ?? SystemConsts.SearchLimit)
        .ToArray();

        if (input.OnlyReturnIds == true)
        {
            var idOnly = limitedIds.Select(id => new GetCustomerResponse
            {

                CustomerId = id.ToString()
            }).ToList();

            return new(idOnly);
        }

        var results = new List<GetCustomerResponse>(limitedIds.Length);
        foreach (var id in limitedIds)
        {
            var customerResponse = await GetCustomerById(new CustomerRequest { CustomerId = id.ToString() });
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
        var customer = await ExecuteWithRetry(() => CustomerClient.getCustomerObjectAsync(Uuid, ParseId(input.CustomerId)));
        if (customer == null)
        {
            throw new PluginApplicationException($"Customer with ID {input.CustomerId} not found. Please check the input and try again");
        }

        var paymentInfo = await ExecuteWithRetry(() => CustomerClient.getPaymentInformationAsync(Uuid, ParseId(input.CustomerId)));
        var createdby = await ExecuteWithRetryAcceptNull(() => CustomerClient.getCreatedByResourceIDAsync(Uuid, ParseId(input.CustomerId)));
        var accountManagerResult = await ExecuteWithRetryAcceptNull(() => CustomerClient.getAccountManagerIDAsync(Uuid, ParseId(input.CustomerId)));
        var addresses = await ExecuteWithRetryAcceptNull(() => CustomerAddressClient.getAllAddressesAsync(Uuid, ParseId(input.CustomerId)))?? Array.Empty<int?>();
        var addressesInfo = new List<GetAddressResponse>();
        var dossier = await ExecuteWithRetryAcceptNull(() => CustomerClient.getDossierAsync(Uuid, ParseId(input.CustomerId)));
        foreach (var addressId in addresses)
        {
            addressesInfo.Add(await GetAddressInfo(addressId));
        }
        return new(customer, paymentInfo, addressesInfo, dossier, accountManagerResult, createdby);
    }

    [Action("Delete customer", Description = "Delete a Plunet customer")]
    public async Task DeleteCustomerById([ActionParameter] CustomerRequest input)
    {
        await ExecuteWithRetry(() => CustomerClient.deleteAsync(Uuid, ParseId(input.CustomerId)));
    }

    [Action("Create customer", Description = "Create a new customer in Plunet")]
    public async Task<GetCustomerResponse> CreateCustomer([ActionParameter] CreateCustomerRequest request)
    {
        if (request.AddressType == null ^ request.Country == null)
            throw new PluginMisconfigurationException(
                "Both address type and country must be specified to create customer with address or not specified at all");

        var customerIdResult = await ExecuteWithRetry(() => CustomerClient.insert2Async(Uuid, new()
        {
            name1 = request.Name1,
            name2 = request.Name2,
            website = request.Website,
            formOfAddress = request.FormOfAddress is null? default : int.Parse(request.FormOfAddress),
            status = request.Status is null ? default : int.Parse(request.Status),
            email = request.Email,
            mobilePhone = request.MobilePhone,
            phone = request.Phone,
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

        var customerId = customerIdResult.ToString();

        if (request.Dossier != null)
        {
            await ExecuteWithRetry(() => CustomerClient.setDossierAsync(Uuid, customerIdResult, request.Dossier));
        }

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
        await ExecuteWithRetry(() => CustomerClient.updateAsync(Uuid, new CustomerIN
        {
            customerID = ParseId(customer.CustomerId),
            name1 = request.Name1,
            name2 = request.Name2,
            website = request.Website,
            formOfAddress = request.FormOfAddress is null? default : int.Parse(request.FormOfAddress),
            status = request.Status is null ? default : int.Parse(request.Status),
            email = request.Email,
            mobilePhone = request.MobilePhone,
            phone = request.Phone,
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

        if (request.Dossier != null)
        {
            await ExecuteWithRetry(() => CustomerClient.setDossierAsync(Uuid, ParseId(customer.CustomerId), request.Dossier));
        }

        var updatedCustomer = await GetCustomerById(customer);

        if (request.AddressType != null)
        {
            if (updatedCustomer.Addresses.Any(x => x.AddressType == request.AddressType))
            {
                await UpdateCustomerAddress(new UpdateCustomerAddressRequest(request, updatedCustomer.Addresses.First(x => x.AddressType == request.AddressType).AddressID));
            }
            else 
            {
                await SetCustomerAddress(new()
                {
                    CustomerId = customer.CustomerId
                }, new(request));
            }   
            return await GetCustomerById(customer);
        }
        else 
        {
            return updatedCustomer;
        }
    }

    [Action("Set customer payment information", Description = "Set customer payment information")]
    public async Task SetCustomerPaymentInfo([ActionParameter] CustomerRequest CustomerId, [ActionParameter] SetPaymentInfoRequest request)
    {
        await ExecuteWithRetry(() => CustomerClient.setPaymentInformationAsync(Uuid, ParseId(CustomerId.CustomerId), new() 
        {
            paymentMethodID = request.PaymentMethodId is null ? default : int.Parse(request.PaymentMethodId) ,
            preselectedTaxID = request.PreselectedTaxId is null ? default : int.Parse(request.PreselectedTaxId),
            accountHolder = request.AccountHolder,
            accountID = request.AccountId is null ? default : int.Parse(request.AccountId),
            salesTaxID = request.SalesTaxId,
            BIC = request.Bic,
            contractNumber = request.ContractNumber,
            debitAccount = request.DebitAccount,
            IBAN = request.Iban
        }));


    }

    private async Task UpdateCustomerAddress(UpdateCustomerAddressRequest request)
    {
         await ExecuteWithRetry(() => CustomerAddressClient.updateAsync(Uuid, new()
        {
            name1 = request.FirstAddressName,
            city = request.City,
            addressType = ParseId(request.AddressType),
            street = request.Street,
            street2 = request.Street2,
            zip = request.ZipCode,
            country = request.Country,
            state = request.State,
            description = request.Description,
            addressID = ParseId(request.AddressId)
        },false));

    }

    //[Action("Set customer address", Description = "Set Plunet customer address")]
    public async Task<SetCustomerAddressResponse> SetCustomerAddress(
        [ActionParameter] CustomerRequest customer, [ActionParameter] SetCustomerAddressRequest request)
    {

        var response = await ExecuteWithRetry(() => CustomerAddressClient.insert2Async(Uuid, ParseId(customer.CustomerId), new()
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

        return new()
        {
            AddressId = response.ToString()
        };
    }

    private async Task<GetAddressResponse> GetAddressInfo(int? addressId)
    {
        var city = await ExecuteWithRetryAcceptNull(() => CustomerAddressClient.getCityAsync(Uuid, (int)addressId));
        var addressType = await ExecuteWithRetry(() => CustomerAddressClient.getAddressTypeAsync(Uuid, (int)addressId));
        var costCenter = await ExecuteWithRetryAcceptNull(() => CustomerAddressClient.getCostCenterAsync(Uuid, (int)addressId));
        var country = await ExecuteWithRetryAcceptNull(() => CustomerAddressClient.getCountryAsync(Uuid, (int)addressId));
        var description = await ExecuteWithRetryAcceptNull(() => CustomerAddressClient.getDescriptionAsync(Uuid, (int)addressId));
        var street1 = await ExecuteWithRetryAcceptNull(() => CustomerAddressClient.getStreetAsync(Uuid, (int)addressId));
        var street2 = await ExecuteWithRetryAcceptNull(() => CustomerAddressClient.getStreet2Async(Uuid, (int)addressId));
        var name = await ExecuteWithRetryAcceptNull(() => CustomerAddressClient.getName1Async(Uuid, (int)addressId));
        var salesTaxType = await ExecuteWithRetryAcceptNull(() => CustomerAddressClient.getSalesTaxationTypeAsync(Uuid, (int)addressId, Language));
        var zip = await ExecuteWithRetryAcceptNull(() => CustomerAddressClient.getZipAsync(Uuid, (int)addressId));
        var state = await ExecuteWithRetryAcceptNull(() => CustomerAddressClient.getStateAsync(Uuid, (int)addressId));

        return new GetAddressResponse
        {
            AddressID = addressId.ToString(),
            AddressType = addressType.ToString(),
            City = city,
            CostCenter = costCenter,
            Country = country,
            Description = description,
            Street = street1,
            Street2 = street2,
            State = state,
            ZipCode = zip,
            FirstAddressName = name,
            SalesTaxType = salesTaxType
            
        };
    }

}