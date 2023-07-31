using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Blackbird.Plugins.Plunet.Connections;

public class ApiKeyConnectionDefinition : IConnectionDefinition
{
    public static IEnumerable<ConnectionProperty> ConnectionProperties => new[]
    {
        new ConnectionProperty(AppConstants.UrlNameKey)
        {
            DisplayName = "Url", Description = "The url to your Plunet instance (https://<your company name>.plunet.com)"
        },
        new(AppConstants.UserNameKey)
        {
            DisplayName = "User name", Description = "Your Plunet username"
        },
        new(AppConstants.PasswordKey)
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
                AppConstants.UrlNameKey, values[AppConstants.UrlNameKey])
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
        using var plunetApiClient = Clients.GetAuthClient(values[AppConstants.UrlNameKey]);
        var uuid = plunetApiClient.loginAsync(values[AppConstants.UserNameKey], values[AppConstants.PasswordKey])
            .GetAwaiter().GetResult();

        return new AuthenticationCredentialsProvider(AuthenticationCredentialsRequestLocation.None,
            AppConstants.ApiKeyName, uuid);
    }
}