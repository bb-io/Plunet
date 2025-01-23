using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.String;

namespace Apps.Plunet.Extensions;

public static class AuthProvidersExtension
{
    public static string GetAuthToken(this IEnumerable<AuthenticationCredentialsProvider> source)
    {
        using var plunetApiClient = Clients.GetAuthClient(source.GetInstanceUrl());
        var uuid = plunetApiClient.loginAsync(source.GetUsername(), source.GetPassword())
            .GetAwaiter().GetResult();

        return uuid;
    }

    public static string GetInstanceUrl(this IEnumerable<AuthenticationCredentialsProvider> source)
    {
        return source.FirstOrDefault(x => x.KeyName == CredsNames.UrlNameKey)
            ?.Value ?? string.Empty;
    }
    public static Uri? GetInstanceUri(this IEnumerable<AuthenticationCredentialsProvider> source)
    {
        return source.FirstOrDefault(x => x.KeyName == CredsNames.UrlNameKey)
            ?.Value.ToUri();
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