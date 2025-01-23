using Apps.Plunet.Actions;
using Apps.Plunet.Models.Order;
using Apps.Plunet.Models.Resource.Request;
using Blackbird.Applications.Sdk.Common.Invocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Plunet.Base;

namespace Tests.Plunet;

[TestClass]
public class ResourceTests : TestBase
{
    public const string resourceName = "Henk";
    public const string updatedResourceName = "Piet";

    [TestMethod]
    public async Task Create_resource_works()
    {
        var actions = new ResourceActions(InvocationContext);

        var result = await actions.CreateResource(new ResourceParameters { Name2 = resourceName });
        Assert.AreEqual(resourceName, result.Name2);
    }

    [TestMethod]
    public async Task Update_resource_works()
    {
        var actions = new ResourceActions(InvocationContext);

        var result = await actions.CreateResource(new ResourceParameters { Name2 = resourceName });
        var updated = await actions.UpdateResource(new ResourceRequest { ResourceId = result.ResourceID }, new ResourceParameters { Name2 = updatedResourceName });
        Assert.AreEqual(updatedResourceName, updated.Name2);
    }
}
