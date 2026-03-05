using Apps.Plunet.Constants;
using Apps.Plunet.Models.Enums;
using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Parsers;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using File = Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems.File;

namespace Apps.Plunet.Strategies;

public abstract class BaseStrategy(IPlunetClientProvider plunet, PickerMode mode) : IPlunetStrategy
{
    protected IPlunetClientProvider Plunet { get; } = plunet;
    protected PickerMode Mode { get; } = mode;

    protected bool IsFileMode => Mode == PickerMode.File;
    protected bool IsFolderMode => Mode == PickerMode.Folder;

    public abstract bool CanHandle(PlunetPath path);

    public abstract Task<IEnumerable<FileDataItem>> HandleAsync(PlunetPath path, CancellationToken ct);

    public abstract IEnumerable<FolderPathItem> ResolveFolderPath(PlunetPath path);

    protected static IEnumerable<FileDataItem> MapToVirtualFolders(int?[]? ids, string nextPrefix)
    {
        return ids?.Where(id => id.HasValue).Select(id => new Folder { Id = $"{nextPrefix}{id}", DisplayName = $"{FolderConstants.DisplayNames.Id}: {id}" }) ?? [];
    }

    protected async Task<IEnumerable<FileDataItem>> ListContentAsync(string folderId, int mainId, int folderType, string[] subPaths, CancellationToken ct)
    {
        var response = await Plunet.DocumentClient.getFileListAsync(Plunet.Uuid, mainId, folderType);
        if (response?.data == null || response.data.Length == 0) return [];

        var normalizedPaths = response.data.Select(p => p.Replace("\\", "/").Trim('/')).ToList();
        string currentPrefix = subPaths.Length > 0 ? string.Join("/", subPaths).TrimEnd('/') + "/" : "";

        var results = new List<FileDataItem>();
        var discoveredFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var path in normalizedPaths.Where(p => string.IsNullOrEmpty(currentPrefix) || p.StartsWith(currentPrefix, StringComparison.OrdinalIgnoreCase)))
        {
            string remainingPath = path.Substring(currentPrefix.Length);
            if (string.IsNullOrWhiteSpace(remainingPath)) continue;

            int firstSlashIndex = remainingPath.IndexOf('/');
            if (firstSlashIndex != -1)
            {
                string folderName = remainingPath.Substring(0, firstSlashIndex);

                if (folderName.StartsWith("$")) continue;

                if (discoveredFolders.Add(folderName))
                {
                    results.Add(new Folder
                    {
                        Id = $"{folderId}/{folderName}",
                        DisplayName = folderName,
                        IsSelectable = IsFolderMode
                    });
                }
            }
            else if (IsFileMode)
            {
                if (remainingPath.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase)) continue;

                if (!remainingPath.Contains('.')) continue;

                results.Add(new File
                {
                    Id = $"{folderId}/{remainingPath}",
                    DisplayName = remainingPath,
                    IsSelectable = IsFileMode
                });
            }
        }

        return results.OrderByDescending(i => i is Folder).ThenBy(i => i.DisplayName);
    }

    protected static int ParseId(string? value, int defaultValue = -1)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        try
        {
            return IntParser.Parse(value, nameof(value)) ?? defaultValue;
        }
        catch (Exception e)
        {
            if (e.Message.Contains("should be an integer"))
                throw new PluginMisconfigurationException($"The given ID is not correct. It should be a number while we received: '{value}'");
            throw;
        }
    }

    protected static int ParseItemId (string segment, string prefix)
    {
        var value = segment.Substring(prefix.Length);
        var parts = value.Split(':');

        return ParseId(parts[0]);
    }

    protected static string GetItemDisplayName(string segment, string prefix)
    {
        var value = segment.Substring(prefix.Length);
        var parts = value.Split(':');

        string index = parts.Length > 1 ? parts[1] : "000";

        return string.Format(FolderConstants.DisplayNames.ItemStringFormat, index);
    }

    protected static string GetJobDisplayName(string segment, string prefix)
    {
        var value = segment.Substring(prefix.Length);
        var parts = value.Split(':');

        var jobId = parts[0];
        var jobType = parts.Length > 1 ? parts[1] : "";

        return string.Format(FolderConstants.DisplayNames.JobFormat, jobType, jobId);
    }
}