using Apps.Plunet.Api;
using Apps.Plunet.Extensions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Plugins.Plunet.DataResource30Service;

namespace Apps.Plunet.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authProviders, CancellationToken cancellationToken)
    {
        try
        {
            var uuid = authProviders.GetAuthToken();
            await using var plunetApiClient = Clients.GetAuthClient(authProviders.GetInstanceUrl());
            await plunetApiClient.logoutAsync(uuid);

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
    }
}