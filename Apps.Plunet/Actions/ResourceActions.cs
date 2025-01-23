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

[ActionList]
public class ResourceActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{

    [Action("Create resource", Description = "Create a new resource")]
    public async Task<ResourceResponse> CreateResource([ActionParameter] ResourceParameters request)
    {
        var response = await ExecuteWithRetry(() => ResourceClient.insertObjectAsync(Uuid, new ResourceIN
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

        return await GetResource(response.ToString());
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
            address.Country = await ExecuteWithRetryAcceptNull(() => ResourceAddressClient.getCountryAsync(Uuid, id));            
            address.City = await ExecuteWithRetryAcceptNull(() => ResourceAddressClient.getCityAsync(Uuid, id));            
            address.Street = await ExecuteWithRetryAcceptNull(() => ResourceAddressClient.getStreetAsync(Uuid, id));            
            address.ZipCode = await ExecuteWithRetryAcceptNull(() => ResourceAddressClient.getZipAsync(Uuid, id));            
            address.State = await ExecuteWithRetryAcceptNull(() => ResourceAddressClient.getStateAsync(Uuid, id));
            addressResponses.Add(type, address);
        }

        var delivery = addressResponses.GetValueOrDefault(1) ?? new AddressResponse();
        var invoice = addressResponses.GetValueOrDefault(2) ?? new AddressResponse();
        var other = addressResponses.GetValueOrDefault(3) ?? new AddressResponse();
        return new(resource, paymentInfo, delivery, invoice, other);
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

        return await GetResource(resource.ResourceId);
    }
}