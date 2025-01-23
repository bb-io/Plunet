using Apps.Plunet.Actions;
using Apps.Plunet.Connections;
using Apps.Plunet.Models.Order;
using Blackbird.Applications.Sdk.Common.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
