using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Plunet.Extensions;

public static class AuthProvidersExtension
{
    public static string GetAuthToken(this IEnumerable<AuthenticationCredentialsProvider> source)
    {
        var plunetApiClient = Clients.GetCached(source.GetInstanceUrl(), Clients.GetAuthClient);
        var uuid = plunetApiClient.loginAsync(source.GetUsername(), source.GetPassword())
            .GetAwaiter().GetResult();

        return uuid;
    }

    public static string GetInstanceUrl(this IEnumerable<AuthenticationCredentialsProvider> source)
    {
        return source.FirstOrDefault(x => x.KeyName == CredsNames.UrlNameKey)
            ?.Value ?? string.Empty;
    }

    public static string GetUsername(this IEnumerable<AuthenticationCredentialsProvider> source)
    {
        return source.FirstOrDefault(x => x.KeyName == CredsNames.UserNameKey)
            ?.Value ?? string.Empty;
    }

    public static string GetPassword(this IEnumerable<AuthenticationCredentialsProvider> source)
    {
        return source.FirstOrDefault(x => x.KeyName == CredsNames.PasswordKey)
            ?.Value ?? string.Empty;
    }
}