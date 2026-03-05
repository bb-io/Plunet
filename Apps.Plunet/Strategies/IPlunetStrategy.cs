using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Plunet.Strategies;

public interface IPlunetStrategy
{
    bool CanHandle(PlunetPath path);

    Task<IEnumerable<FileDataItem>> HandleAsync(PlunetPath path, CancellationToken ct);

    IEnumerable<FolderPathItem> ResolveFolderPath(PlunetPath path);
}