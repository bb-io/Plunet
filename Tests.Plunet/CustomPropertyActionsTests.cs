using Apps.Plunet.Actions;
using Apps.Plunet.Models.CustomProperties;
using Newtonsoft.Json;
using Tests.Plunet.Base;

namespace Tests.Plunet;

[TestClass]
internal class CustomPropertyActionsTests : TestBase
{
    [TestMethod]
    public async Task GetTextModule_IsSuccess()
    {
        // Arrange
        var action = new CustomPropertyActions(InvocationContext);
        var request = new TextModuleRequest
        {
            Flag = "[XTM-User-ID]",
            UsageArea = "2",
            MainId = "67"
        };

        // Act
        var response = await action.GetTextmodule(request);

        // Assert
        Assert.IsNotNull(response);
        Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
    }
}
