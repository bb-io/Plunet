using Apps.Plunet.Constants;
using Apps.Plunet.Models.Enums;
using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using static Apps.Plunet.Constants.FolderTypeConstants;

namespace Apps.Plunet.Strategies;

public class ResourceStrategy(IPlunetClientProvider plunet, PickerMode mode) : BaseStrategy(plunet, mode), IPlunetStrategy
{
    public override bool CanHandle(PlunetPath path) => path.RootSegment.StartsWith(FolderConstants.EntityPrefixes.Resource);

    public override async Task<IEnumerable<FileDataItem>> HandleAsync(PlunetPath path, CancellationToken ct)
    {
        int mainId = ParseId(path.RootSegment.Substring(FolderConstants.EntityPrefixes.Resource.Length));
        return await ListContentAsync(path.Raw, mainId, Resource, path.Segments, ct);
    }

    public override IEnumerable<FolderPathItem> ResolveFolderPath(PlunetPath path)
    {
        int mainId = ParseId(path.RootSegment.Substring(FolderConstants.EntityPrefixes.Resource.Length));

        var breadcrumbs = new List<FolderPathItem>
        {
            new() { Id = FolderConstants.VirtualRoots.Home, DisplayName = FolderConstants.DisplayNames.Home },
            new() { Id = FolderConstants.VirtualRoots.Resource, DisplayName = FolderConstants.DisplayNames.Resource },
            new() { Id = path.RootSegment, DisplayName = $"{FolderConstants.DisplayNames.Id}: {mainId}" }
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
}