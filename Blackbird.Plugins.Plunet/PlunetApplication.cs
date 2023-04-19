using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet;

public class PlunetApplication : IApplication
{
    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }

    public string Name { get => "Plunet plugin"; set{} }
}