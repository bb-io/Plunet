using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Enums;
using Apps.Plunet.Strategies;
using Apps.Plunet.Utils;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Plunet.DataSourceHandlers;

public class FolderPickerDataSourceHandler(InvocationContext context) : PlunetInvocable(context), IAsyncFileDataSourceItemHandler
{
    public async Task<IEnumerable<FileDataItem>> GetFolderContentAsync(FolderContentDataSourceContext context, CancellationToken cancellationToken)
    {
        var path = PathParser.Parse(context.FolderId);
        var provider = new StrategyProvider(FfClientProvider, PickerMode.Folder);

        var strategy = provider.GetStrategy(path);
        if (strategy is null) return [];

        return await strategy.HandleAsync(path, cancellationToken);
    }

    public Task<IEnumerable<FolderPathItem>> GetFolderPathAsync(FolderPathDataSourceContext context, CancellationToken cancellationToken)
    {
        var path = PathParser.Parse(context.FileDataItemId);
        var provider = new StrategyProvider(FfClientProvider, PickerMode.File);

        var strategy = provider.GetStrategy(path);
        
        return strategy is null 
            ? Task.FromResult<IEnumerable<FolderPathItem>>([]) 
            : Task.FromResult(strategy.ResolveFolderPath(path));
    }
}