using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Tests.Plunet.Base;

public class TestBase
{
    protected IEnumerable<AuthenticationCredentialsProvider> Creds { get; set; }

    protected InvocationContext InvocationContext { get; set; }

    protected FileManager FileManager { get; set; }

    protected TestBase()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        Creds = config.GetSection("ConnectionDefinition").GetChildren().Select(x => new AuthenticationCredentialsProvider(x.Key, x.Value)).ToList();
        var folderLocation = config.GetSection("TestFolder").Value;

        InvocationContext = new InvocationContext
        {
            AuthenticationCredentialsProviders = Creds,
        };

        FileManager = new FileManager(folderLocation);
    }

    protected static void PrintJsonResult(object result)
    {
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    protected static void PrintDataHandlerResult(IEnumerable<DataSourceItem> items)
    {
        Console.WriteLine($"Count: {items.Count()}");

        foreach (var item in items)
            Console.WriteLine($"{item.Value} - {item.DisplayName}");
    }
}
