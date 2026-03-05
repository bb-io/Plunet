using Apps.Plunet.Constants;
using Apps.Plunet.Models.Enums;
using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using static Apps.Plunet.Constants.FolderConstants;

namespace Apps.Plunet.Strategies;

public class RequestStrategy(FfClientProvider ffClientProvider, PickerMode mode) : BaseStrategy(ffClientProvider, mode), IPlunetStrategy
{
    public override bool CanHandle(FfPath path) => path.RootSegment.StartsWith(EntityPrefixes.Request);

    public override async Task<IEnumerable<FileDataItem>> HandleAsync(FfPath path, CancellationToken ct)
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

    public override IEnumerable<FolderPathItem> ResolveFolderPath(FfPath path)
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

    public override PathInfo ResolvePathInfo(FfPath path)
    {
        if (path.Segments.Length == 0) throw new PluginMisconfigurationException($"The path '{path.Raw}' is not supported here.");
        
        int mainId = ParseId(path.RootSegment.Substring(EntityPrefixes.Request.Length));

        return new PathInfo(
            mainId,
            FolderTypeConstants.Request[path.Segments[0]],
            string.Join('/', path.Segments.Skip(1))
        );
    }
}