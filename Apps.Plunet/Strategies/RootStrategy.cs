using Apps.Plunet.Constants;
using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Plunet.Strategies;

public class RootStrategy : IPlunetStrategy
{
    public bool CanHandle(FfPath path) => path.Raw == FolderConstants.VirtualRoots.Home;

    public Task<IEnumerable<FileDataItem>> HandleAsync(FfPath path, CancellationToken ct)
    {
        var result = FolderConstants.RootFolders.Select(kvp => new Folder
        {
            Id = kvp.Key,
            DisplayName = kvp.Value,
            IsSelectable = false
        });

        return Task.FromResult<IEnumerable<FileDataItem>>(result);
    }

    public IEnumerable<FolderPathItem> ResolveFolderPath(FfPath path)
    {
        return [            
            new FolderPathItem { Id = FolderConstants.VirtualRoots.Home, DisplayName = FolderConstants.DisplayNames.Home },
        ];
    }

    public PathInfo ResolvePathInfo(FfPath path)
    {
        throw new PluginMisconfigurationException($"The path '{path.Raw}' is not supported here.");
    }
}