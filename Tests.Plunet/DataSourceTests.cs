using Apps.Plunet.Actions;
using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Apps.Plunet.Models.Order;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Plunet.Models.Job;
using Tests.Plunet.Base;

namespace Tests.Plunet;

[TestClass]
public class DataSourceTests : TestBase
{
    [TestMethod]
    public async Task Templates_returns_values()
    {
        var handler = new TemplateDataHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);
        foreach(var item in result)
        {
            Console.WriteLine($"{item.Key}: {item.Value}");
        }
        Assert.IsTrue(result.Count > 0);
    }

    [TestMethod]
    public async Task Customers_returns_values()
    {
        var handler = new CustomerIdDataHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Key}: {item.Value}");
        }
        Assert.IsTrue(result.Count > 0);
    }

    //[TestMethod]
    //public async Task Project_manager_returns_values()
    //{
    //    var handler = new ProjectManagerIdDataHandler(InvocationContext);

    //    var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);
    //    foreach (var item in result)
    //    {
    //        Console.WriteLine($"{item.Key}: {item.Value}");
    //    }
    //    Assert.IsTrue(result.Count > 0);
    //}

    [TestMethod]
    public async Task Countries_returns_values()
    {
        var handler = new CountryDataSourceHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Key}: {item.Value}");
        }
        Assert.IsTrue(result.Count > 0);
    }
    
    [TestMethod]
    public async Task LanguageNameDataHandler_returns_values()
    {
        var handler = new LanguageNameDataHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext(), CancellationToken.None);
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Key}: {item.Value}");
        }
        Assert.IsTrue(result.Count > 0);
    }

    [TestMethod]
    public async Task JobPriceUnitDataHandler_returns_values()
    {
        var handler = new JobPriceUnitDataHandler(InvocationContext, new()
        {
            Service = "Proofreading"
        });
        
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);
        var dataSourceItems = result as DataSourceItem[] ?? result.ToArray();
        
        foreach (var item in dataSourceItems)
        {
            Console.WriteLine($"{item.Value}: {item.DisplayName}");
        }
        Assert.IsTrue(dataSourceItems.Any());
    }

    [TestMethod]
    public async Task ServiceNameDataHandler_returns_values()
    {
        var handler = new ServiceNameDataHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);
      
        var json = System.Text.Json.JsonSerializer.Serialize(result);
        Console.WriteLine(json);

        Assert.IsNotNull(result);
    }


    [TestMethod]
    public async Task PayableDataSourceHandler_returns_values()
    {
        var handler = new PayableDataSourceHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.DisplayName}");
        }

        Assert.IsNotNull(result);
    }
}
