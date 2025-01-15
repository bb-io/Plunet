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
using Blackbird.Plugins.Plunet.DataCustomFields30;
using Blackbird.Plugins.Plunet.DataResource30Service;
using Result = Blackbird.Plugins.Plunet.DataResource30Service.Result;

namespace Apps.Plunet.Actions;

[ActionList]
public class ResourceActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{

    [Action("Create resource", Description = "Create a new, empty resource dataset")]
    public async Task<IntegerResult> CreateResource([ActionParameter] CreateResourceRequest request)
    {
        var response = await ExecuteWithRetry<IntegerResult>(async () => 
        await ResourceClient.insertAsync(Uuid, int.Parse(request.WorkingStatus)));
        return response;
    }


    [Action("Search resources", Description = "Search for specific resources based on specific criteria")]
    public async Task<SearchResponse<ResourceResponse>> SearchResources([ActionParameter] SearchResourcesRequest input)
    {
        var response = await ExecuteWithRetry<IntegerArrayResult>(async () => await ResourceClient.searchAsync(Uuid,
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

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        if (response.data is null)
        {
            return new();
        }

        var results = new List<ResourceResponse>();
        foreach (var id in response.data.Where(x => x.HasValue).Take(input.Limit ?? SystemConsts.SearchLimit))
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
                var textModuleResult = await ExecuteWithRetry<TextmoduleResult>(async () =>
                    await CustomFieldsClient.getTextmoduleAsync(Uuid, input.Flag,
                        resourceUsageArea, ParseId(resource.ResourceID), Language));
                if (textModuleResult.statusMessage == ApiResponses.Ok &&
                    textModuleResult.data.stringValue.Equals(input.TextModuleValue))
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
            throw new("No resources found with the given text module value");
        }

        return new(result.Items.FirstOrDefault(), result.TotalCount);
    }

    // Get resource price lists (optional source+target)

    [Action("Get resource", Description = "Get details of a specific resource")]
    public async Task<ResourceResponse> GetResource(
        [ActionParameter] [DataSource(typeof(ResourceIdDataHandler))] [Display("Resource ID")]
        string resourceId)
    {
        var response = await ExecuteWithRetry<ResourceResult>(async () =>
            await ResourceClient.getResourceObjectAsync(Uuid, ParseId(resourceId)));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        var paymentInfoResponse = await ExecuteWithRetry<PaymentInfoResult>(async () =>
            await ResourceClient.getPaymentInformationAsync(Uuid, ParseId(resourceId)));

        if (paymentInfoResponse.statusMessage != ApiResponses.Ok)
            throw new(paymentInfoResponse.statusMessage);

        var addresses = await ExecuteWithRetry<DataResourceAddress30Service.IntegerArrayResult>(async () =>
            await ResourceAddressClient.getAllAddressesAsync(Uuid, ParseId(resourceId)));

        var addressResponse = new AddressResponse();
        foreach (var id in addresses.data)
        {
            var countryResponse = await ExecuteWithRetry<DataResourceAddress30Service.StringResult>(async () =>
                await ResourceAddressClient.getCountryAsync(Uuid, id.Value));
            addressResponse.Countries.Add(countryResponse.data);
            
            var cityResponse = await ExecuteWithRetry<DataResourceAddress30Service.StringResult>(async () =>
                await ResourceAddressClient.getCityAsync(Uuid, id.Value));
            addressResponse.Cities.Add(cityResponse.data);
            
            var streetResponse = await ExecuteWithRetry<DataResourceAddress30Service.StringResult>(async () =>
                await ResourceAddressClient.getStreetAsync(Uuid, id.Value));
            addressResponse.Streets.Add(streetResponse.data);
            
            var zipCodeResponse = await ExecuteWithRetry<DataResourceAddress30Service.StringResult>(async () =>
                await ResourceAddressClient.getZipAsync(Uuid, id.Value));
            addressResponse.ZipCodes.Add(zipCodeResponse.data);
            
            var stateResponse = await ExecuteWithRetry<DataResourceAddress30Service.StringResult>(async () =>
                await ResourceAddressClient.getStateAsync(Uuid, id.Value));
            addressResponse.States.Add(stateResponse.data);
        }

        addressResponse.FirstCountry = addressResponse.Countries.FirstOrDefault() ?? string.Empty;
        return new(response.data, paymentInfoResponse.data)
        {
            AddressData = addressResponse
        };
    }

    [Action("Update resource", Description = "Update a specific resource with new details")]
    public async Task<ResourceResponse> UpdateResource([ActionParameter] UpdateResourceRequest request)
    {
        var formOfAddress = string.IsNullOrEmpty(request.FormOfAddress)
            ? (await ExecuteWithRetry<IntegerResult>(async () =>
                await ResourceClient.getFormOfAddressAsync(Uuid, ParseId(request.ResourceId)))).data
            : ParseId(request.FormOfAddress);

        var status = string.IsNullOrEmpty(request.Status)
            ? (await ExecuteWithRetry<IntegerResult>(async () =>
                await ResourceClient.getStatusAsync(Uuid, ParseId(request.ResourceId)))).data
            : ParseId(request.Status);

        var response = await ExecuteWithRetry<Result>(async () =>
            await ResourceClient.updateAsync(Uuid, new ResourceIN
            {
                resourceID = ParseId(request.ResourceId),
                academicTitle = request.AcademicTitle,
                costCenter = request.CostCenter ?? string.Empty,
                currency = request.Currency ?? string.Empty,
                email = request.Email ?? string.Empty,
                externalID = request.ExternalId ?? string.Empty,
                fax = request.Fax ?? string.Empty,
                formOfAddress = formOfAddress,
                fullName = request.FullName ?? string.Empty,
                mobilePhone = request.MobilePhone ?? string.Empty,
                name1 = request.Name1 ?? string.Empty,
                name2 = request.Name2 ?? string.Empty,
                opening = request.Opening ?? string.Empty,
                phone = request.Phone ?? string.Empty,
                resourceType = 0,
                skypeID = request.SkypeId ?? string.Empty,
                status = status,
                supervisor1 = request.Supervisor1 ?? string.Empty,
                supervisor2 = request.Supervisor2 ?? string.Empty,
                userId = ParseId(request.UserId),
                website = request.Website ?? string.Empty,
                workingStatus = ParseId(request.WorkingStatus)
            }, false));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        return await GetResource(request.ResourceId);
    }

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

            if (result.statusMessage.Contains("session-UUID used is invalid") && attempts < maxRetries)
            {
                await Task.Delay(delay);
                await RefreshAuthToken();
                attempts++;
                continue;
            }

            return (T)result;
        }
    }

    private async Task<T> ExecuteWithRetry<T>(Func<Task<Blackbird.Plugins.Plunet.DataCustomFields30.Result>> func,
        int maxRetries = 10, int delay = 1000)
        where T : Blackbird.Plugins.Plunet.DataCustomFields30.Result
    {
        var attempts = 0;
        while (true)
        {
            Blackbird.Plugins.Plunet.DataCustomFields30.Result? result;
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

            if (result.statusMessage.Contains("session-UUID used is invalid"))
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

    private async Task<T> ExecuteWithRetry<T>(Func<Task<DataResourceAddress30Service.IntegerArrayResult>> func,
        int maxRetries = 10, int delay = 1000)
        where T : DataResourceAddress30Service.IntegerArrayResult
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();

            if (result.statusMessage == ApiResponses.Ok)
            {
                return (T)result;
            }

            if (result.statusMessage.Contains("session-UUID used is invalid"))
            {
                if (attempts < maxRetries)
                {
                    await Task.Delay(delay);
                    await RefreshAuthToken();
                    attempts++;
                    continue;
                }

                throw new($"No more retries left. Last error: {result.statusMessage}, Session UUID used is invalid.");
            }

            return (T)result;
        }
    }

    private async Task<T> ExecuteWithRetry<T>(Func<Task<DataResourceAddress30Service.StringResult>> func,
        int maxRetries = 10, int delay = 1000)
        where T : DataResourceAddress30Service.StringResult
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();

            if (result.statusMessage == ApiResponses.Ok)
            {
                return (T)result;
            }

            if (result.statusMessage.Contains("session-UUID used is invalid"))
            {
                if (attempts < maxRetries)
                {
                    await Task.Delay(delay);
                    await RefreshAuthToken();
                    attempts++;
                    continue;
                }

                throw new($"No more retries left. Last error: {result.statusMessage}, Session UUID used is invalid.");
            }

            return (T)result;
        }
    }
}