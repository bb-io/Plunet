using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Plugins.Plunet.Api;
using Blackbird.Plugins.Plunet.Constants;

namespace Blackbird.Plugins.Plunet.Connections;

public class ApiKeyConnectionDefinition : IConnectionDefinition
{
    public static IEnumerable<ConnectionProperty> ConnectionProperties => new[]
    {
        new ConnectionProperty(CredsNames.UrlNameKey)
        {
            DisplayName = "Url", Description = "The url to your Plunet instance (https://<your company name>.plunet.com)"
        },
        new(CredsNames.UserNameKey)
        {
            DisplayName = "Username", Description = "Your Plunet username"
        },
        new(CredsNames.PasswordKey)
        {
            DisplayName = "Password", Description = "Your Plunet password", Sensitive = true
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        return new[]
        {
            CreateAuthorizationCredentialsProvider(values),
            new(AuthenticationCredentialsRequestLocation.None,
                CredsNames.UrlNameKey, values[CredsNames.UrlNameKey].TrimEnd('/'))
        };
    }

    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new()
        {
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = ConnectionProperties,
            ConnectionUsage = ConnectionUsage.Webhooks,
            Name = "Plunet Connection"
        }
    };

    private AuthenticationCredentialsProvider CreateAuthorizationCredentialsProvider(Dictionary<string, string> values)
    {
        using var plunetApiClient = Clients.GetAuthClient(values[CredsNames.UrlNameKey]);
        var uuid = plunetApiClient.loginAsync(values[CredsNames.UserNameKey], values[CredsNames.PasswordKey])
            .GetAwaiter().GetResult();

        return new AuthenticationCredentialsProvider(AuthenticationCredentialsRequestLocation.None,
            CredsNames.ApiKeyName, uuid);
    }
}