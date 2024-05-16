using Apps.Plunet.Constants;
using Blackbird.Plugins.Plunet.DataQuote30Service;

namespace Apps.Plunet;

public static class RetryHandler
{
    public static async Task<T> ExecuteWithRetry<T>(Func<Task<Result>> func, int maxRetries = 5, int delay = 1000)
        where T : Result
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();
            
            if(result.statusMessage == ApiResponses.Ok)
            {
                return (T)result;
            }
            
            if(result.statusMessage.Contains("session-UUID used is invalid") && attempts < maxRetries)
            {
                await Task.Delay(delay);
                attempts++;
                continue;
            }
            
            throw new InvalidOperationException($"Failed to execute function after {attempts} attempts. Last error: {result.statusMessage}");
        }
    }
}