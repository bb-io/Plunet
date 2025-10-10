using Apps.Plunet.Actions;
using Apps.Plunet.Models.Resource.Request;
using Newtonsoft.Json;
using Tests.Plunet.Base;

namespace Tests.Plunet;

[TestClass]
public class ResourceTests : TestBase
{
    public const string resourceName = "Henk";
    public const string updatedResourceName = "Piet";

    public const string deliveryCountry = "Albania";
    public const string updatedDeliveryCountry = "Germany";

    public const string invoiceStreet1 = "Dorpstraat";
    public const string updatedInvoiceStreet1 = "Kerkstraat";

    public const string contractNumber = "01249";
    public const string updatedContractNumber = "124910";

    [TestMethod]
    public async Task GetResource_IsSuccess()
    {
        // Arrange
        var actions = new ResourceActions(InvocationContext);

        // Act
        var result = await actions.GetResource("1");

        // Assert
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task CreateResource_WithResourceType_IsSuccess()
    {
        // Arrange
        var actions = new ResourceActions(InvocationContext);
        var request = new ResourceParameters
        {
            Name2 = "Jesse",
            Phone = "123456",
            ResourceType = "1"
        };

        // Act
        var result = await actions.CreateResource(request);

        // Assert
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.AreEqual(request.Name2, result.Name2);
        Assert.AreEqual(request.Phone, result.Phone);
        Assert.AreEqual(request.ResourceType, result.ResourceType);
    }

    [TestMethod]
    public async Task CreateResource_WithoutResourceType_IsSuccess()
    {
        // Arrange
        var actions = new ResourceActions(InvocationContext);
        var request = new ResourceParameters
        {
            Name2 = "Skyler",
            Phone = "11111",
            DeliveryStreet = "308 Negra Arroyo Lane"
        };

        // Act
        var result = await actions.CreateResource(request);

        // Assert
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.AreEqual(request.Name2, result.Name2);
        Assert.AreEqual(request.Phone, result.Phone);
        Assert.AreEqual("0", result.ResourceType);
    }

    [TestMethod]
    public async Task UpdateResource_IsSuccess()
    {
        // Arrange
        var actions = new ResourceActions(InvocationContext);
        var createResourceRequest = new ResourceParameters
        {
            Name2 = "Walter",
            DeliveryCountry = "United States",
            ContractNumber = "call saul",
            ResourceType = "3"
        };
        var updateResourceRequest = new ResourceParameters
        {
            Name2 = "Walter",
            DeliveryCountry = "Mexico",
            InvoiceStreet = "better call saul"
        };

        // Act
        var created = await actions.CreateResource(createResourceRequest);
        var updated = await actions.UpdateResource(new ResourceRequest { ResourceId = created.ResourceID }, updateResourceRequest);

        // Assert
        Console.WriteLine(JsonConvert.SerializeObject(updated, Formatting.Indented));
        Assert.AreEqual(created.Name2, updated.Name2);
        Assert.AreEqual(updateResourceRequest.DeliveryCountry, updated.DeliveryAddress.Country);
        Assert.AreEqual(createResourceRequest.ResourceType, updated.ResourceType);
    }

    [TestMethod]
    public async Task SearchResources_ReturnValue()
    {
        var action = new ResourceActions(InvocationContext);
        var input = new SearchResourcesRequest { Limit = 1000, OnlyReturnIds = true,
        //SelectedPropertyValueIds= ["2","74","108"],
            //PropertyType = "3",
            PropertyNameEnglish = "Test",
            
        };
        var result = await action.SearchResources(input);

        Console.WriteLine(JsonConvert.SerializeObject(result));
        Assert.IsNotNull(result);
    }
}
