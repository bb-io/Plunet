using System.Text.Json.Serialization;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Blackbird.Plugins.Plunet.Connections;

public class ApiKeyConnectionDefinition : IConnectionDefinition
{
    private const string ApiKeyName = "UUID";
    private const string UserNameKey = "userName";
    private const string PasswordKey = "password";

    public static IEnumerable<ConnectionProperty> ConnectionProperties => new[]
    {
        new ConnectionProperty(UserNameKey) {DisplayName = "User name"},
        new ConnectionProperty(PasswordKey) {DisplayName = "Password", Sensitive = true}
    };
    
    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        return new[] {CreateAuthorizationCredentialsProvider(values)};
    }

    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new ConnectionPropertyGroup()
        {
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = ConnectionProperties,
            ConnectionUsage = ConnectionUsage.Webhooks,
            Name = "Plunet Connection"
        }
    };

    private AuthenticationCredentialsProvider CreateAuthorizationCredentialsProvider(Dictionary<string, string> values)
    {
        string uuid;
        using var plunetApiClient = new PlunetAPIService.PlunetAPIClient();
        {
            uuid = plunetApiClient.loginAsync(values[UserNameKey], values[PasswordKey]).GetAwaiter().GetResult();
        }

        return new AuthenticationCredentialsProvider(AuthenticationCredentialsRequestLocation.None, ApiKeyName, uuid);
    }
}