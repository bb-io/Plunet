using Apps.Plunet.Constants;
using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.CustomProperties;
using Apps.Plunet.Models.Resource.Request;
using Apps.Plunet.Models.Resource.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataCustomFields30;
using Blackbird.Plugins.Plunet.DataResource30Service;
using Result = Blackbird.Plugins.Plunet.DataResource30Service.Result;

namespace Apps.Plunet.Actions;

[ActionList]
public class ResourceActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{
    [Action("Search resources", Description = "Search for specific resources based on specific criteria")]
    public async Task<ListResourceResponse> SearchResources([ActionParameter] SearchResourcesRequest input)
    {
        var response = await ExecuteWithRetry<IntegerArrayResult>(async () => await ResourceClient.searchAsync(Uuid,
            new Blackbird.Plugins.Plunet.DataResource30Service.SearchFilter_Resource()
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
            return new()
            {
                Resources = Enumerable.Empty<ResourceResponse>()
            };
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

            return new()
            {
                Resources = textModuleResources
            };
        }

        return new()
        {
            Resources = results
        };
    }

    [Action("Find resource by text module", Description = "Find resources by text module")]
    public async Task<ResourceResponse> FindResourceByTextModule([ActionParameter] FindByTextModuleRequest request)
    {
        var result = await SearchResources(new SearchResourcesRequest
            { TextModuleValue = request.TextModuleValue, Flag = request.Flag });

        if (result.Resources.Any() == false)
        {
            throw new("No resources found with the given text module value");
        }

        return result.Resources.First();
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

        return new(response.data, paymentInfoResponse.data);
    }

    private async Task<T> ExecuteWithRetry<T>(Func<Task<Result>> func, int maxRetries = 10, int delay = 1000)
        where T : Result
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();

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
            var result = await func();

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

                throw new($"No more retries left. Last error: {result.statusMessage}, Session UUID used is invalid.");
            }

            return (T)result;
        }
    }
}