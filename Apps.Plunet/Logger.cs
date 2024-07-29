using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Plunet;

public static class Logger
{
    public static async Task LogAsync<T>(T obj) where T : class
    {
        var restRequest = new RestRequest(string.Empty, Method.Post)
            .WithJsonBody(obj);
        
        var restClient = new RestClient("https://webhook.site/47e2048f-ca56-4e79-9604-b48868194894");
        await restClient.ExecuteAsync(restRequest);
    }

    public static async Task LogAsync(Exception e)
    {
        await LogAsync(new
        {
            Exception = e.Message,
            e.StackTrace,
            InnerException = e.InnerException?.Message,
            Type = e.GetType().Name
        });
    }
}