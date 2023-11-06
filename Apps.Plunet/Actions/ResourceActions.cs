﻿using Apps.Plunet.Models.Resource.Request;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
using Blackbird.Plugins.Plunet.Api;
using Blackbird.Plugins.Plunet.Constants;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Invocables;
using Blackbird.Plugins.Plunet.Models.Payable.Response;
using Blackbird.Plugins.Plunet.Models.Resource.Response;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class ResourceActions : PlunetInvocable
{
    public ResourceActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Search resources", Description = "Search for specific reasources based on specific criteria")]
    public async Task<ListResourceResponse> SearchResources([ActionParameter] SearchResourcesRequest input)
    {

        var statuses = new int?[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var response = await ResourceClient.searchAsync(Uuid, new DataResource30Service.SearchFilter_Resource() 
        {
            contact_resourceID = IntParser.Parse(input.ContactId, nameof(input.ContactId)) ?? -1,
            email = input.Email ?? "",
            name1 = input.Name1 ?? "",
            name2 = input.Name2 ?? "",
            resourceType = IntParser.Parse(input.ResourceType, nameof(input.ResourceType)) ?? -1,
            resourceStatus = IntParser.Parse(input.Status, nameof(input.Status)) ?? -1,
            sourceLanguageCode = input.SourceLanguageCode ?? "",
            targetLanguageCode = input.TargetLanguageCode ?? "",
            workingStatus = IntParser.Parse(input.WorkingStatus, nameof(input.WorkingStatus)) ?? -1,
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

        return new()
        {
            Resources = result
        };
    }

    // Get resource price lists (optional source+target)


    [Action("Get resource", Description = "Get details of a specific resource")]
    public async Task<ResourceResponse> GetResource(
        [ActionParameter] [Display("Resource ID")]
        string resourceId)
    {
        
        var intId = IntParser.Parse(resourceId, nameof(resourceId))!.Value;
        var response = await ResourceClient.getResourceObjectAsync(Uuid, intId);

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        var paymentInfoResponse = await ResourceClient.getPaymentInformationAsync(Uuid, intId);

        if (paymentInfoResponse.statusMessage != ApiResponses.Ok)
            throw new(paymentInfoResponse.statusMessage);

        return new(response.data, paymentInfoResponse.data);
    }
}