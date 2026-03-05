using Apps.Plunet.Constants;
using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Plunet.Strategies;

public class RootStrategy : IPlunetStrategy
{
    public bool CanHandle(PlunetPath path) => path.Raw == FolderConstants.VirtualRoots.Home;

    public Task<IEnumerable<FileDataItem>> HandleAsync(PlunetPath path, CancellationToken ct)
    {
        var result = FolderConstants.RootFolders.Select(kvp => new Folder
        {
            Id = kvp.Key,
            DisplayName = kvp.Value,
            IsSelectable = false
        });

        return Task.FromResult<IEnumerable<FileDataItem>>(result);
    }

    public IEnumerable<FolderPathItem> ResolveFolderPath(PlunetPath path)
    {
        return [            
            new FolderPathItem { Id = FolderConstants.VirtualRoots.Home, DisplayName = FolderConstants.DisplayNames.Home },
        ];
    }
}