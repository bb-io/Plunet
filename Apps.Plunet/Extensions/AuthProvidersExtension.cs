﻿using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Plugins.Plunet.Constants;

namespace Blackbird.Plugins.Plunet.Extensions;

public static class AuthProvidersExtension
{
    public static string GetAuthToken(this IEnumerable<AuthenticationCredentialsProvider> source) =>
        source.FirstOrDefault(x => x.KeyName == CredsNames.ApiKeyName)
            ?.Value ?? string.Empty;

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
    
    public static async Task Logout(this IEnumerable<AuthenticationCredentialsProvider> source)
    {
        var uuid = source.GetAuthToken();
      
        await using var plunetApiClient = new PlunetAPIService.PlunetAPIClient(source.GetInstanceUri());
        await plunetApiClient.logoutAsync(uuid);
    }
}