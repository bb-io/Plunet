using Blackbird.Applications.Sdk.Common.Authentication;

namespace Blackbird.Plugins.Plunet.Extensions;

public static class AuthProvidersExtension
{
    public static string GetAuthToken(this IEnumerable<AuthenticationCredentialsProvider> source) =>
        source.FirstOrDefault(x => x.KeyName == AppConstants.ApiKeyName)
            ?.Value ?? string.Empty;

    public static string GetInstanceUrl(this IEnumerable<AuthenticationCredentialsProvider> source)
    {
        return source.FirstOrDefault(x => x.KeyName == AppConstants.UrlNameKey)
            ?.Value ?? string.Empty;
    }
    
    public static async Task Logout(this IEnumerable<AuthenticationCredentialsProvider> source)
    {
        var uuid = source.GetAuthToken();
        await using var plunetApiClient = new PlunetAPIService.PlunetAPIClient();
        await plunetApiClient.logoutAsync(uuid);
    }
}