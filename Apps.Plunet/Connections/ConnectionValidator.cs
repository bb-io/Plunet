using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Plugins.Plunet.Api;
using Blackbird.Plugins.Plunet.Extensions;

namespace Blackbird.Plugins.Plunet.Connections;

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
            
            var customerClient = Clients.GetCustomerClient(authProviders.GetInstanceUrl());

            var allStatuses = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            await customerClient
                .getAllCustomerObjects2Async(uuid, Array.ConvertAll(allStatuses, i => (int?)i));
            
            await authProviders.Logout();

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