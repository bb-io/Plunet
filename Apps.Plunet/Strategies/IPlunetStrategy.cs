using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Plunet.Strategies;

public interface IPlunetStrategy
{
    bool CanHandle(FfPath path);

    Task<IEnumerable<FileDataItem>> HandleAsync(FfPath path, CancellationToken ct);

    IEnumerable<FolderPathItem> ResolveFolderPath(FfPath path);

    PathInfo ResolvePathInfo(FfPath path);
}