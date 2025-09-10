using Apps.Plunet.Actions;
using Apps.Plunet.Models.Order;
using Tests.Plunet.Base;

namespace Tests.Plunet;

[TestClass]
public class OrderTests : TestBase
{
    [TestMethod]
    public async Task Create_order_by_template_works()
    {
        var actions = new OrderActions(InvocationContext);

        var result = await actions.CreateOrderByTemplate("1", new CreateOrderByTemplateRequest { CustomerId = "1", ProjectManagerId = "1", ProjectName = "Test order" });
        Assert.IsNotNull(result.OrderId);
    }

    [TestMethod]
    public async Task SearchOrders_IsSuccess()
    {
        var actions = new OrderActions(InvocationContext);

        var result = await actions.SearchOrders(new SearchOrderInput { OnlyReturnIds=true, Limit=10 });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
        Console.WriteLine(json);

        Assert.IsNotNull(result);
    }
}
