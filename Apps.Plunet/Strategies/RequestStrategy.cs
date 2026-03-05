using Apps.Plunet.Constants;
using Apps.Plunet.Models.Enums;
using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using static Apps.Plunet.Constants.FolderConstants;

namespace Apps.Plunet.Strategies;

public class RequestStrategy(IPlunetClientProvider plunet, PickerMode mode) : BaseStrategy(plunet, mode), IPlunetStrategy
{
    public override bool CanHandle(PlunetPath path) => path.RootSegment.StartsWith(EntityPrefixes.Request);

    public override async Task<IEnumerable<FileDataItem>> HandleAsync(PlunetPath path, CancellationToken ct)
    {
        int mainId = ParseId(path.RootSegment.Substring(EntityPrefixes.Request.Length));

        if (path.Segments.Length == 0)
        {
            return FolderTypeConstants.Request.Select(folder => new Folder { DisplayName = folder.Key, Id = $"{path.Raw}/{folder.Key}" });
        }

        if (FolderTypeConstants.Request.TryGetValue(path.Segments[0], out var folderType))
        {
            return await ListContentAsync(path.Raw, mainId, folderType, path.Segments.Skip(1).ToArray(), ct);
        }

        return [];
    }

    public override IEnumerable<FolderPathItem> ResolveFolderPath(PlunetPath path)
    {
        int mainId = ParseId(path.RootSegment.Substring(EntityPrefixes.Request.Length));

        var breadcrumbs = new List<FolderPathItem>
        {
            new() { Id = VirtualRoots.Home, DisplayName = DisplayNames.Home },
            new() { Id = VirtualRoots.Request, DisplayName = DisplayNames.Request },
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
}