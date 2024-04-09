using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.Plunet;

public class PlunetApplication : IApplication, ICategoryProvider
{
    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.TranslationBusinessManagement];
        set { }
    }
    
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