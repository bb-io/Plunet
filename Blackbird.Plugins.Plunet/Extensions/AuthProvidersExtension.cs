using Blackbird.Applications.Sdk.Common.Authentication;

namespace Blackbird.Plugins.Plunet.Extensions;

public static class AuthProvidersExtension
{
    public static string GetAuthToken(this IEnumerable<AuthenticationCredentialsProvider> source) =>
        source.FirstOrDefault(x => x.CredentialsRequestLocation == AuthenticationCredentialsRequestLocation.None)
            ?.Value ?? string.Empty;
    
    public static async Task Logout(this IEnumerable<AuthenticationCredentialsProvider> source)
    {
        var uuid = source.GetAuthToken();
        using var plunetApiClient = new PlunetAPIService.PlunetAPIClient();
        await plunetApiClient.logoutAsync(uuid);
    }
}