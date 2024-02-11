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

namespace Apps.Plunet.Actions;

[ActionList]
public class ResourceActions : PlunetInvocable
{
    public ResourceActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Search resources", Description = "Search for specific resources based on specific criteria")]
    public async Task<ListResourceResponse> SearchResources([ActionParameter] SearchResourcesRequest input)
    {
        var response = await ResourceClient.searchAsync(Uuid, new Blackbird.Plugins.Plunet.DataResource30Service.SearchFilter_Resource() 
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
        });

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        if (response.data is null)
            return new()
            {
                Resources = Enumerable.Empty<ResourceResponse>()
            };

        var ids = response.data.Where(x => x.HasValue)
            .Select(x => GetResource(x!.Value.ToString()))
            .ToArray();

        var result = await Task.WhenAll(ids);
        
        if (input.Flag is not null)
        {
            var textModuleResources = new List<ResourceResponse>();
            int resourceUsageArea = 2;
            
            foreach (var resource in result)
            {
                var textModuleResult = await CustomFieldsClient.getTextmoduleAsync(Uuid, input.Flag,
                    resourceUsageArea, ParseId(resource.ResourceID), Language);
                if (textModuleResult.statusMessage == ApiResponses.Ok)
                {
                    if (textModuleResult.data.stringValue.Equals(input.TextModuleValue))
                    {
                        textModuleResources.Add(resource);
                    }
                }
            }
            
            return new()
            {
                Resources = textModuleResources
            };
        }
        
        return new()
        {
            Resources = result
        };
    }
    
    [Action("Find resource by text module", Description = "Find resources by text module")]
    public async Task<ResourceResponse> FindResourceByTextModule([ActionParameter] FindResourceByTextModuleRequest request)
    {
        var result = await SearchResources(new SearchResourcesRequest { TextModuleValue = request.TextModuleValue, Flag = request.Flag });
        
        if(result.Resources.Any() == false)
        {
            throw new("No resources found with the given text module value");
        }
        
        return result.Resources.First();
    }

    // Get resource price lists (optional source+target)

    [Action("Get resource", Description = "Get details of a specific resource")]
    public async Task<ResourceResponse> GetResource(
        [ActionParameter][DataSource(typeof(ResourceIdDataHandler))] [Display("Resource ID")]
        string resourceId)
    {        
        var response = await ResourceClient.getResourceObjectAsync(Uuid, ParseId(resourceId));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        var paymentInfoResponse = await ResourceClient.getPaymentInformationAsync(Uuid, ParseId(resourceId));

        if (paymentInfoResponse.statusMessage != ApiResponses.Ok)
            throw new(paymentInfoResponse.statusMessage);

        return new(response.data, paymentInfoResponse.data);
    }
}