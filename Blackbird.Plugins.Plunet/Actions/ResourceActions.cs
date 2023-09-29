using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Invocables;
using Blackbird.Plugins.Plunet.Models.Resource.Response;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class ResourceActions : PlunetInvocable
{
    public ResourceActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }
    
    [Action("Get payable resource", Description = "Get resource ID of a specific payable")]
    public async Task<ResourceResponse> GetPayableResource(
        [ActionParameter] [Display("Payable ID")] string payableId)
    {
        var uuid = Creds.GetAuthToken();

        await using var client = Clients.GetPayableClient(Creds.GetInstanceUrl());

        var intId = IntParser.Parse(payableId, nameof(payableId))!.Value;
        var response = await client.getResourceIDAsync(uuid, intId);

        await Creds.Logout();

        if (response.statusMessage != "OK")
            throw new(response.statusMessage);

        return await GetResource(response.data.ToString());
    }    
    
    [Action("Get resource", Description = "Get details of a specific resource")]
    public async Task<ResourceResponse> GetResource(
        [ActionParameter] [Display("Resource ID")] string resourceId)
    {
        var uuid = Creds.GetAuthToken();

        await using var client = Clients.GetResourceClient(Creds.GetInstanceUrl());

        var intId = IntParser.Parse(resourceId, nameof(resourceId))!.Value;
        var response = await client.getResourceObjectAsync(uuid, intId);

        await Creds.Logout();

        if (response.statusMessage != "OK")
            throw new(response.statusMessage);

        return new(response.data);
    }
}