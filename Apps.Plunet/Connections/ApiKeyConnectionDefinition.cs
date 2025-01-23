using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Plunet.Connections;

public class ApiKeyConnectionDefinition : IConnectionDefinition
{
    public static IEnumerable<ConnectionProperty> ConnectionProperties => new[]
    {
        new ConnectionProperty(CredsNames.UrlNameKey)
        {
            DisplayName = "URL", Description = "The url to your Plunet instance (https://<your company name>.plunet.com)"
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
        return new AuthenticationCredentialsProvider[]
        {
            new(CredsNames.UrlNameKey, values[CredsNames.UrlNameKey].TrimEnd('/')),
            new(CredsNames.UserNameKey, values[CredsNames.UserNameKey]),
            new(CredsNames.PasswordKey, values[CredsNames.PasswordKey]),
        };
    }

    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new()
        {
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = ConnectionProperties,
            Name = "Plunet Connection"
        }
    };
}