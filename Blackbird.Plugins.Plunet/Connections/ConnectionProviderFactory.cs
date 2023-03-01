using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Connections;

public class ConnectionProviderFactory : IConnectionProviderFactory
{
    public IEnumerable<IConnectionProvider> Create()
    {
        yield return new ConnectionProvider();
    }
}