using Blackbird.Plugins.Plunet.DataResource30Service;

namespace Apps.Plunet.Extensions;

public static class ResourceExtensions
{
    public static string ToDisplayName(this Resource resource)
    {
        return $"{resource.name2} {resource.name1}".Trim();
    }
}