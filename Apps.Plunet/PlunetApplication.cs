using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet;

public class PlunetApplication : IApplication
{
    public string Name
    {
        get => "Plunet";
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}