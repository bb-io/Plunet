using Tests.Plunet.Base;
using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Apps.Plunet.Models.Job;

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

    [TestMethod]
    public async Task Project_manager_returns_values()
    {
        var handler = new ProjectManagerIdDataHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Key}: {item.Value}");
        }
        Assert.IsTrue(result.Count > 0);
    }

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

    [TestMethod]
    public async Task JobRoundDataHandler_ReturnsJobRoundIds()
    {
        // Arrange
        var job = new GetJobRequest { JobId = "268", ProjectType = "3" };
        var handler = new JobRoundDataHandler(InvocationContext, job);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        // Assert
        foreach (var item in result)
            Console.WriteLine($"{item.Value}: {item.DisplayName}");

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task RoundResourceDataHandler_ReturnsJobRoundResourceIds()
    {
        // Arrange
        var round = new JobRoundRequest { JobRoundId = "404" };
        var handler = new RoundResourceDataHandler(InvocationContext, round);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        // Assert
        foreach (var item in result)
            Console.WriteLine($"{item.Value}: {item.DisplayName}");

        Assert.IsNotNull(result);
    }
}
