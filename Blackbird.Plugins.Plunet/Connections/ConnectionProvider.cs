using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Blackbird.Plugins.Plunet.Connections;

public class ConnectionProvider : IConnectionProvider
{
    public AuthenticationCredentialsProvider Create(IDictionary<string, string> connectionValues)
    {
        return new AuthenticationCredentialsProvider(AuthenticationCredentialsRequestLocation.None, string.Empty, string.Empty);
    }

    public string ConnectionName  =>  "Blackbird";

    
    public IEnumerable<string> ConnectionProperties  => new [] {"url", "username", "password"};
}