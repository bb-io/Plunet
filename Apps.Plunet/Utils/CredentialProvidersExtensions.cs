using System;
using Apps.Plunet.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;

namespace Apps.Plunet.Utils;

public static class CredentialProvidersExtensions
{
    public static string GetUrl(this IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var url = creds.Get(CredsNames.UrlNameKey).Value;
        if(url.EndsWith('/'))
        {
            url = url.Substring(0, url.Length - 1);
        }

        return url;
    }
}
