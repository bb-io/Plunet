using Apps.Plunet.Constants;
using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.CustomProperties;
using Apps.Plunet.Models.Resource.Request;
using Apps.Plunet.Models.Resource.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataResource30Service;

namespace Apps.Plunet.Actions;

[ActionList("Resources")]
public class ResourceActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{

    [Action("Create resource", Description = "Create a new resource")]
    public async Task<ResourceResponse> CreateResource([ActionParameter] ResourceParameters request)
    {
        var resourceId = await ExecuteWithRetry(() => ResourceClient.insertObjectAsync(Uuid, new ResourceIN
        {
            academicTitle = request.AcademicTitle,
            costCenter = request.CostCenter ?? string.Empty,
            currency = request.Currency ?? string.Empty,
            email = request.Email ?? string.Empty,
            externalID = request.ExternalId ?? string.Empty,
            fax = request.Fax ?? string.Empty,
            formOfAddress = ParseId(request.FormOfAddress, 1),
            fullName = request.FullName ?? string.Empty,
            mobilePhone = request.MobilePhone ?? string.Empty,
            name1 = request.Name1 ?? string.Empty,
            name2 = request.Name2 ?? string.Empty,
            opening = request.Opening ?? string.Empty,
            phone = request.Phone ?? string.Empty,
            resourceType = 0,
            skypeID = request.SkypeId ?? string.Empty,
            status = ParseId(request.Status, 4),
            supervisor1 = request.Supervisor1 ?? string.Empty,
            supervisor2 = request.Supervisor2 ?? string.Empty,
            userId = ParseId(request.UserId, 0),
            website = request.Website ?? string.Empty,
            workingStatus = ParseId(request.WorkingStatus, 2)
        }));

        await UpdatePaymentMethod(resourceId, request);
        await UpdateAddresses(resourceId, request);

        return await GetResource(resourceId.ToString()); 
    }

    [Action("Search resources", Description = "Search for specific resources based on specific criteria")]
    public async Task<SearchResponse<ResourceResponse>> SearchResources([ActionParameter] SearchResourcesRequest input)
    {
        var response = await ExecuteWithRetryAcceptNull(() => ResourceClient.searchAsync(Uuid,
            new SearchFilter_Resource()
            {
                contact_resourceID = ParseId(input.ContactId),
                email = input.Email ?? string.Empty,
                name1 = input.Name1 ?? string.Empty,
                name2 = input.Name2 ?? string.Empty,
                resourceType = ParseId(input.ResourceType),
                resourceStatus = ParseId(input.Status),
                sourceLanguageCode = input.SourceLanguageCode ?? string.Empty,
                targetLanguageCode = input.TargetLanguageCode ?? string.Empty,
                workingStatus = ParseId(input.WorkingStatus),
            }));

        if (response is null)
        {
            return new();
        }

        var results = new List<ResourceResponse>();
        foreach (var id in response.Where(x => x.HasValue).Take(input.Limit ?? SystemConsts.SearchLimit))
        {
            var resourceResponse = await GetResource(id!.Value.ToString());
            results.Add(resourceResponse);
        }

        if (input.Flag is not null)
        {
            var textModuleResources = new List<ResourceResponse>();
            int resourceUsageArea = 2;

            foreach (var resource in results)
            {
                var textmodule = await ExecuteWithRetryAcceptNull(() => CustomFieldsClient.getTextmoduleAsync(Uuid, input.Flag, resourceUsageArea, ParseId(resource.ResourceID), Language));
                if (textmodule is not null && textmodule.stringValue.Equals(input.TextModuleValue))
                {
                    textModuleResources.Add(resource);
                }
            }

            return new(textModuleResources);
        }

        return new(results);
    }

    [Action("Find resource", Description = "Find a specific resource based on specific criteria")]
    public async Task<FindResponse<ResourceResponse>> FindResource([ActionParameter] SearchResourcesRequest request)
    {
        var result = await SearchResources(request);
        return new(result.Items.FirstOrDefault(), result.TotalCount);
    }

    [Action("Find resource by text module", Description = "Find resources by text module")]
    public async Task<FindResponse<ResourceResponse>> FindResourceByTextModule(
        [ActionParameter] FindByTextModuleRequest request)
    {
        var result = await SearchResources(new SearchResourcesRequest
            { TextModuleValue = request.TextModuleValue, Flag = request.Flag });

        if (result.Items.Any() == false)
        {
            throw new PluginApplicationException("No resources found with the given text module value");
        }

        return new(result.Items.FirstOrDefault(), result.TotalCount);
    }

    // Get resource price lists (optional source+target)

    [Action("Get resource", Description = "Get details of a specific resource")]
    public async Task<ResourceResponse> GetResource(
        [ActionParameter] [DataSource(typeof(ResourceIdDataHandler))] [Display("Resource ID")]
        string resourceId)
    {
        var resource = await ExecuteWithRetry(() => ResourceClient.getResourceObjectAsync(Uuid, ParseId(resourceId)));
        var paymentInfo = await ExecuteWithRetry(() => ResourceClient.getPaymentInformationAsync(Uuid, ParseId(resourceId)));
        var addresses = await ExecuteWithRetry(() => ResourceAddressClient.getAllAddressesAsync(Uuid, ParseId(resourceId)));

        Dictionary<int, AddressResponse> addressResponses = new Dictionary<int, AddressResponse>();        
        foreach (var id in addresses.Where(x => x.HasValue).Select(x => x.Value))
        {
            var address = new AddressResponse();            
            var type = await ExecuteWithRetry(() => ResourceAddressClient.getAddressTypeAsync(Uuid, id));
            if (addressResponses.ContainsKey(type)) continue;
            address.Name1 = await ExecuteWithRetryAcceptNull(() => ResourceAddressClient.getName1Async(Uuid, id));
            address.Name2 = await ExecuteWithRetryAcceptNull(() => ResourceAddressClient.getName2Async(Uuid, id));
            address.Description = await ExecuteWithRetryAcceptNull(() => ResourceAddressClient.getDescriptionAsync(Uuid, id));            
            address.Country = await ExecuteWithRetryAcceptNull(() => ResourceAddressClient.getCountryAsync(Uuid, id));
            address.State = await ExecuteWithRetryAcceptNull(() => ResourceAddressClient.getStateAsync(Uuid, id));
            address.City = await ExecuteWithRetryAcceptNull(() => ResourceAddressClient.getCityAsync(Uuid, id));            
            address.Street = await ExecuteWithRetryAcceptNull(() => ResourceAddressClient.getStreetAsync(Uuid, id));
            address.Street2 = await ExecuteWithRetryAcceptNull(() => ResourceAddressClient.getStreet2Async(Uuid, id));
            address.ZipCode = await ExecuteWithRetryAcceptNull(() => ResourceAddressClient.getZipAsync(Uuid, id));  
            address.Office = await ExecuteWithRetryAcceptNull(() => ResourceAddressClient.getOfficeAsync(Uuid, id));
            addressResponses.Add(type, address);
        }

        var delivery = addressResponses.GetValueOrDefault(1) ?? new AddressResponse();
        var invoice = addressResponses.GetValueOrDefault(2) ?? new AddressResponse();
        return new(resource, paymentInfo, delivery, invoice);
    }

    [Action("Update resource", Description = "Update a specific resource with new details")]
    public async Task<ResourceResponse> UpdateResource([ActionParameter] ResourceRequest resource, [ActionParameter] ResourceParameters request)
    {
        var formOfAddress = string.IsNullOrEmpty(request.FormOfAddress)
            ? (await ExecuteWithRetryAcceptNull(() => ResourceClient.getFormOfAddressAsync(Uuid, ParseId(resource.ResourceId))))
            : ParseId(request.FormOfAddress);

        var status = string.IsNullOrEmpty(request.Status)
            ? (await ExecuteWithRetryAcceptNull(() => ResourceClient.getStatusAsync(Uuid, ParseId(resource.ResourceId))))
            : ParseId(request.Status);

        var userId = string.IsNullOrEmpty(request.UserId)
            ? (await ExecuteWithRetryAcceptNull(() => ResourceClient.getUserIdAsync(Uuid, ParseId(resource.ResourceId))))
            : ParseId(request.UserId);

        var workingStatus = string.IsNullOrEmpty(request.WorkingStatus)
            ? (await ExecuteWithRetryAcceptNull(() => ResourceClient.getWorkingStatusAsync(Uuid, ParseId(resource.ResourceId))))
            : ParseId(request.WorkingStatus);

        await ExecuteWithRetry(() =>
            ResourceClient.updateAsync(Uuid, new ResourceIN
            {
                resourceID = ParseId(resource.ResourceId),
                academicTitle = request.AcademicTitle,
                costCenter = request.CostCenter ?? string.Empty,
                currency = request.Currency ?? string.Empty,
                email = request.Email ?? string.Empty,
                externalID = request.ExternalId ?? string.Empty,
                fax = request.Fax ?? string.Empty,
                formOfAddress = formOfAddress ?? 3,
                fullName = request.FullName ?? string.Empty,
                mobilePhone = request.MobilePhone ?? string.Empty,
                name1 = request.Name1 ?? string.Empty,
                name2 = request.Name2 ?? string.Empty,
                opening = request.Opening ?? string.Empty,
                phone = request.Phone ?? string.Empty,
                resourceType = 0,
                skypeID = request.SkypeId ?? string.Empty,
                status = status ?? 4,
                supervisor1 = request.Supervisor1 ?? string.Empty,
                supervisor2 = request.Supervisor2 ?? string.Empty,
                userId = userId ?? 0,
                website = request.Website ?? string.Empty,
                workingStatus = workingStatus ?? 2,
            }, false));

        await UpdatePaymentMethod(ParseId(resource.ResourceId), request);
        await UpdateAddresses(ParseId(resource.ResourceId), request);

        return await GetResource(resource.ResourceId);
    }

    private async Task UpdatePaymentMethod(int resourceId, ResourceParameters request)
    {
        await ExecuteWithRetry(() => ResourceClient.setPaymentInformationAsync(Uuid, resourceId, new PaymentInfo
        {
            accountHolder = request.AccountHolder,
            accountID = ParseId(request.AccountId, 0),
            BIC = request.Bic,
            contractNumber = request.ContractNumber,
            debitAccount = request.DebitAccount,
            IBAN = request.Iban,
            paymentMethodID = ParseId(request.PaymentMethodId, 1),
            preselectedTaxID = ParseId(request.PreselectdTaxId, 0),
            salesTaxID = request.SalesTaxId,
        }));
    }

    private async Task UpdateAddresses(int resourceId, ResourceParameters request)
    {
        int deliveryId = 0;
        int invoiceId = 0;
        var addresses = await ExecuteWithRetry(() => ResourceAddressClient.getAllAddressesAsync(Uuid, resourceId));

        foreach (var id in addresses.Where(x => x.HasValue).Select(x => x.Value))
        {
            var type = await ExecuteWithRetry(() => ResourceAddressClient.getAddressTypeAsync(Uuid, id));
            if (type == 1)
                deliveryId = id;
            else if (type == 2)
                invoiceId = id;
        }

        if (deliveryId == 0)
        {
            deliveryId = await ExecuteWithRetry(() => ResourceAddressClient.insertAsync(Uuid, resourceId));
            await ExecuteWithRetry(() => ResourceAddressClient.setAddressTypeAsync(Uuid, 1, deliveryId));
        }

        if (invoiceId == 0)
        {
            invoiceId = await ExecuteWithRetry(() => ResourceAddressClient.insertAsync(Uuid, resourceId));
            await ExecuteWithRetry(() => ResourceAddressClient.setAddressTypeAsync(Uuid, 2, invoiceId));
        }

        if (request.DeliveryName1 is not null) await ExecuteWithRetry(() => ResourceAddressClient.setName1Async(Uuid, request.DeliveryName1, deliveryId));
        if (request.DeliveryName2 is not null) await ExecuteWithRetry(() => ResourceAddressClient.setName2Async(Uuid, request.DeliveryName2, deliveryId));
        if (request.DeliveryDescription is not null) await ExecuteWithRetry(() => ResourceAddressClient.setDescriptionAsync(Uuid, request.DeliveryDescription, deliveryId));
        if (request.DeliveryCountry is not null) await ExecuteWithRetry(() => ResourceAddressClient.setCountryAsync(Uuid, request.DeliveryCountry, deliveryId));
        if (request.DeliveryState is not null) await ExecuteWithRetry(() => ResourceAddressClient.setStateAsync(Uuid, request.DeliveryState, deliveryId));
        if (request.DeliveryCity is not null) await ExecuteWithRetry(() => ResourceAddressClient.setCityAsync(Uuid, request.DeliveryCity, deliveryId));
        if (request.DeliveryStreet is not null) await ExecuteWithRetry(() => ResourceAddressClient.setStreetAsync(Uuid, request.DeliveryStreet, deliveryId));
        if (request.DeliveryStreet2 is not null) await ExecuteWithRetry(() => ResourceAddressClient.setStreet2Async(Uuid, request.DeliveryStreet2, deliveryId));
        if (request.DeliveryZipCode is not null) await ExecuteWithRetry(() => ResourceAddressClient.setZipAsync(Uuid, request.DeliveryZipCode, deliveryId));
        if (request.DeliveryOffice is not null) await ExecuteWithRetry(() => ResourceAddressClient.setOfficeAsync(Uuid, request.DeliveryOffice, deliveryId));

        if (request.InvoiceName1 is not null) await ExecuteWithRetry(() => ResourceAddressClient.setName1Async(Uuid, request.InvoiceName1, invoiceId));
        if (request.InvoiceName2 is not null) await ExecuteWithRetry(() => ResourceAddressClient.setName2Async(Uuid, request.InvoiceName2, invoiceId));
        if (request.InvoiceDescription is not null) await ExecuteWithRetry(() => ResourceAddressClient.setDescriptionAsync(Uuid, request.InvoiceDescription, invoiceId));
        if (request.InvoiceCountry is not null) await ExecuteWithRetry(() => ResourceAddressClient.setCountryAsync(Uuid, request.InvoiceCountry, invoiceId));
        if (request.InvoiceState is not null) await ExecuteWithRetry(() => ResourceAddressClient.setStateAsync(Uuid, request.InvoiceState, invoiceId));
        if (request.InvoiceCity is not null) await ExecuteWithRetry(() => ResourceAddressClient.setCityAsync(Uuid, request.InvoiceCity, invoiceId));
        if (request.InvoiceStreet is not null) await ExecuteWithRetry(() => ResourceAddressClient.setStreetAsync(Uuid, request.InvoiceStreet, invoiceId));
        if (request.InvoiceStreet2 is not null) await ExecuteWithRetry(() => ResourceAddressClient.setStreet2Async(Uuid, request.InvoiceStreet2, invoiceId));
        if (request.InvoiceZipCode is not null) await ExecuteWithRetry(() => ResourceAddressClient.setZipAsync(Uuid, request.InvoiceZipCode, invoiceId));
        if (request.InvoiceOffice is not null) await ExecuteWithRetry(() => ResourceAddressClient.setOfficeAsync(Uuid, request.InvoiceOffice, invoiceId));
    }
}