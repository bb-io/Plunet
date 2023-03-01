using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.Models.Login;

namespace Blackbird.Plugins.Plunet;

[ActionList]
public class AuthActions
{
    [Action]
    public LoginResponse GetAuthToken(string userName, string password, AuthenticationCredentialsProvider authProvider)
    {
        using var plunetApiClient = new PlunetAPIService.PlunetAPIClient();
        var uuidResult = plunetApiClient.loginAsync(userName, password).GetAwaiter().GetResult();
        return new LoginResponse
        {
            UUID = uuidResult
        };
    }
    
    [Action]
    public void Logout(string userName, string password, AuthenticationCredentialsProvider authProvider, [ActionParameter]string uuid)
    {
        using var plunetApiClient = new PlunetAPIService.PlunetAPIClient();
        plunetApiClient.logoutAsync(uuid).GetAwaiter().GetResult();
    }
}