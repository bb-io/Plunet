using Apps.Plunet.Constants;
using Apps.Plunet.Models.Enums;
using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using static Apps.Plunet.Constants.FolderConstants;

namespace Apps.Plunet.Strategies;

public class ResourceStrategy(FfClientProvider ffClientProvider, PickerMode mode) : BaseStrategy(ffClientProvider, mode), IPlunetStrategy
{
    public override bool CanHandle(FfPath path) => path.RootSegment.StartsWith(FolderConstants.EntityPrefixes.Resource);

    public override async Task<IEnumerable<FileDataItem>> HandleAsync(FfPath path, CancellationToken ct)
    {
        int mainId = ParseId(path.RootSegment.Substring(EntityPrefixes.Resource.Length));
        return await ListContentAsync(path.Raw, mainId, FolderTypeConstants.Resource, path.Segments, ct);
    }

    public override IEnumerable<FolderPathItem> ResolveFolderPath(FfPath path)
    {
        int mainId = ParseId(path.RootSegment.Substring(EntityPrefixes.Resource.Length));

        var breadcrumbs = new List<FolderPathItem>
        {
            new() { Id = VirtualRoots.Home, DisplayName = DisplayNames.Home },
            new() { Id = VirtualRoots.Resource, DisplayName = DisplayNames.Resource },
            new() { Id = path.RootSegment, DisplayName = $"{DisplayNames.Id}: {mainId}" }
        };

        if (path.Segments.Length == 0) return breadcrumbs;

        string current = path.RootSegment;

        foreach (var segment in path.Segments)
        {
            current = $"{current}/{segment}";
            breadcrumbs.Add(new FolderPathItem { Id = current, DisplayName = segment });
        }

        return breadcrumbs;
    }

    public override PathInfo? ResolvePathInfo(FfPath path)
    {
        int mainId = ParseId(path.RootSegment.Substring(EntityPrefixes.Resource.Length));

        return new PathInfo(
            mainId,
            FolderTypeConstants.Resource,
            string.Join('/', path.Segments)
        );
    }
}