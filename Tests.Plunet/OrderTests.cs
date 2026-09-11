using Apps.Plunet.Actions;
using Apps.Plunet.Models.Order;
using Newtonsoft.Json;
using Tests.Plunet.Base;

namespace Tests.Plunet;

[TestClass]
public class OrderTests : TestBase
{
    [TestMethod]
    public async Task Create_order_by_template_works()
    {
        // Arrange
        var actions = new OrderActions(InvocationContext);

        // Act
        var result = await actions.CreateOrderByTemplate("5", new CreateOrderByTemplateRequest
        {
            CustomerId = "2",
            ProjectManagerId = "1",
            Deadline = DateTime.Now.AddDays(7),
        });

        // Assert
        Assert.IsNotNull(result.OrderId);
    }
    
    [TestMethod]
    public async Task CreateOrderConfirmation_ReturnsConfirmationFilePath()
    {
        // Arrange
        var actions = new OrderActions(InvocationContext);
        var orderInput = new OrderRequest { OrderId = "573" };
        var createInput = new CreateOrderConfirmationRequest
        {
            FormatId = "0",
            TemplateName = "English"
        };

        // Act
        var result = await actions.CreateOrderConfirmation(orderInput, createInput);

        // Assert
        Console.WriteLine(result.OrderConfirmationFileLocation);
        Assert.IsNotNull(result.OrderConfirmationFileLocation);
        Assert.IsNotEmpty(result.OrderConfirmationFileLocation);
    }

    [TestMethod]
    public async Task SearchOrders_IsSuccess()
    {
        // Arrange
        var actions = new OrderActions(InvocationContext);

        // Act
        var result = await actions.SearchOrders(new SearchOrderInput 
        { 
            DateRelation = "5",
            DateFrom = new DateTime(2024, 06, 19),
            DateTo = new DateTime(2025, 06, 21),
            ItemStatus = ["8", "2"]
        });

        // Assert
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetOrder_IsSuccess()
    {
        // Arrange
        var actions = new OrderActions(InvocationContext);
        var request = new OrderRequest { OrderId = "623" };

        // Act
        var result = await actions.GetOrder(request);

        // Assert
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsNotNull(result);
    }
}
