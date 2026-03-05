using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.Models.Job;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
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

    [TestMethod]
    public async Task FolderPickerDataSourceHandler_ReturnsVirtualFolders()
    {
        // Arrange
        var handler = new FolderPickerDataSourceHandler(InvocationContext);

        // Act
        var result = await handler.GetFolderContentAsync(new FolderContentDataSourceContext { }, CancellationToken.None);

        // Assert
        foreach (var item in result)
            Console.WriteLine($"{item.Id} | {item.DisplayName} | {item.IsSelectable}");

        Assert.IsNotNull(result);
        Assert.AreEqual(6, result.Count());
    }

    [TestMethod]
    public async Task FolderPickerDataSourceHandler_ReturnsContent()
    {
        // Arrange
        var handler = new FolderPickerDataSourceHandler(InvocationContext);

        // Act
        var result = await handler.GetFolderContentAsync(new FolderContentDataSourceContext { FolderId = "c:136" }, CancellationToken.None);

        // Assert
        foreach (var item in result)
            Console.WriteLine($"{item.Id}: {item.DisplayName} | {item.IsSelectable}");

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task FilePickerDataSourceHandler_ReturnsContent()
    {
        // Arrange
        var handler = new FilePickerDataSourceHandler(InvocationContext);

        // Act
        //var result = await handler.GetFolderContentAsync(new FolderContentDataSourceContext { FolderId = "q:104/vqi/qi:32:001/vqij/qij:1:INT/!_Out" }, CancellationToken.None);
        var result = await handler.GetFolderContentAsync(new FolderContentDataSourceContext { FolderId = "o:663/voi/oi:499:001/voij/oij:303:REV/!_Out" }, CancellationToken.None);

        // Assert
        foreach (var item in result)
            Console.WriteLine($"{item.Id} | {item.DisplayName} | {item.IsSelectable}");

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task FilePickerDataSourceHandler_ReturnsFolderPath()
    {
        // Arrange
        var handler = new FilePickerDataSourceHandler(InvocationContext);

        // Act
        var result = await handler.GetFolderPathAsync(new FolderPathDataSourceContext { FileDataItemId = "q:104/vqi/qi:32:001/vqij/qij:1:INT" }, CancellationToken.None);

        // Assert
        foreach (var item in result)
            Console.WriteLine($"{item.Id} | {item.DisplayName}");

        Assert.IsNotNull(result);
    }
}