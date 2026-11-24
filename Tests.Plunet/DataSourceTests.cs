using Tests.Plunet.Base;
using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.Models.Item;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Apps.Plunet.Models.Job;

namespace Tests.Plunet;

[TestClass]
public class DataSourceTests : TestBase
{
    [TestMethod]
    public async Task TemplateDataHandler_ReturnsTemplates()
    {
        // Arrange
        var handler = new TemplateDataHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);
        
        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task CustomerIdDataHandler_ReturnsCustomerIds()
    {
        // Arrange
        var handler = new CustomerIdDataHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);
        
        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
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
    public async Task JobPriceUnitDataHandler_ReturnsJobPriceUnits()
    {
        // Arrange
        var handler = new JobPriceUnitDataHandler(InvocationContext, new()
        {
            Service = "Proofreading"
        });
        
        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);
        
        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task ServiceNameDataHandler_ReturnsServiceNames()
    {
        // Arrange
        var handler = new ServiceNameDataHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);
      
        // Assert
        PrintDataHandlerResult(result);
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
    public async Task QuoteIdDataHandler_ReturnsQuoteIds()
    {
        // Arrange
        var handler = new QuoteIdDataHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task OrderIdDataHandler_ReturnsOrderIds()
    {
        // Arrange
        var handler = new OrderIdDataHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task ItemPriceUnitDataHandler_ReturnsPriceUnits()
    {
        // Arrange
        var request = new ItemPriceUnitRequest { Service = "Interpreting" };
        var handler = new ItemPriceUnitDataHandler(InvocationContext, request);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task ItemPriceUnitDescriptionHandler_ReturnsPriceUnitDescriptions()
    {
        // Arrange
        var request = new ItemPriceUnitRequest { Service = "Interpreting" };
        var handler = new ItemPriceUnitDescriptionHandler(InvocationContext, request);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task JobTypeDataHandler_ReturnsJobTypes()
    {
        // Arrange
        var handler = new JobTypeDataHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task JobTypeOptionDataHandler_ReturnsJobTypeOptions()
    {
        // Arrange
        var handler = new JobTypeOptionDataHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task QuoteTemplateDataHandler_ReturnsQuoteTemplates()
    {
        // Arrange
        var handler = new QuoteTemplateDataHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task ResourceIdDataHandler_ReturnsResourceIds()
    {
        // Arrange
        var handler = new ResourceIdDataHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "Sky" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task WorkflowIdDataHandler_ReturnsWorkflowIds()
    {
        // Arrange
        var handler = new WorkflowIdDataHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
}
