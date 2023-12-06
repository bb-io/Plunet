using Apps.Plunet.Api;
using Apps.Plunet.Extensions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Plunet.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authProviders, CancellationToken cancellationToken)
    {
        try
        {
            var uuid = authProviders.GetAuthToken();

            if (uuid == "refused")
                return new()
                {
                    IsValid = false,
                    Message = "Wrong username of password"
                };

            return new()
            {
                IsValid = true
            };
        }
        catch (Exception ex)
        {
            return new()
            {
                IsValid = false,
                Message = ex.Message
            };
        }
        finally
        {
            await authProviders.Logout();
        }
    }
}