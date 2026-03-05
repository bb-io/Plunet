using Apps.Plunet.Constants;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Enums;
using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using static Apps.Plunet.Constants.FolderConstants;

namespace Apps.Plunet.Strategies;

public class QuoteStrategy(FfClientProvider ffClientProvider, PickerMode mode) : BaseStrategy(ffClientProvider, mode), IPlunetStrategy
{
    public override bool CanHandle(FfPath path) => path.RootSegment.StartsWith(EntityPrefixes.Quote);

    public override async Task<IEnumerable<FileDataItem>> HandleAsync(FfPath path, CancellationToken ct)
    {
        int mainId = ParseId(path.RootSegment.Substring(EntityPrefixes.Quote.Length));

        if (path.Segments.Length == 0)
        {
            return new[]
            {
                new Folder { DisplayName = "Items", Id = $"{path.Raw}/{QuoteRoute.Items}" },
                new Folder { DisplayName = "Jobs (item independent)", Id = $"{path.Raw}/{QuoteRoute.IndependentJobs}" },
            }.Concat(
                FolderTypeConstants.Quote.Select(folder => new Folder 
                { 
                    DisplayName = folder.Key, Id = $"{path.Raw}/{folder.Key}", IsSelectable = IsFolderMode 
                })
            );
        }

        if (FolderTypeConstants.Quote.TryGetValue(path.Segments[0], out var folderType))
        {
            return await ListContentAsync(path.Raw, mainId, folderType, path.Segments.Skip(1).ToArray(), ct);
        }

        return path.Segments[0] switch
        {
            QuoteRoute.Items => await HandleQuoteItems(path, mainId, ct),
            QuoteRoute.IndependentJobs => await HandleItemIndependentJobs(path, mainId, ct),
            _ => []
        };
    }

    public override IEnumerable<FolderPathItem> ResolveFolderPath(FfPath path)
    {
        int mainId = ParseId(path.RootSegment.Substring(EntityPrefixes.Quote.Length));

        var breadcrumbs = new List<FolderPathItem>
        {
            new() { Id = VirtualRoots.Home, DisplayName = DisplayNames.Home },
            new() { Id = VirtualRoots.Quote, DisplayName = DisplayNames.Quote },
            new() { Id = path.RootSegment, DisplayName = $"{DisplayNames.Id}: {mainId}" }
        };

        if (path.Segments.Length == 0) return breadcrumbs;

        string current = path.RootSegment;

        foreach (var segment in path.Segments)
        {
            current = $"{current}/{segment}";

            string display = segment switch
            {
                QuoteRoute.Items => "Items",
                QuoteRoute.IndependentJobs => "Jobs (item independent)",
                QuoteRoute.Item.Jobs => "Jobs (item dependent)",
                _ when segment.StartsWith(QuoteRoute.Item.Prefix) => GetItemDisplayName(segment, QuoteRoute.Item.Prefix),
                _ when segment.StartsWith(QuoteRoute.Job.IndependentPrefix) => GetJobDisplayName(segment, QuoteRoute.Job.IndependentPrefix),
                _ when segment.StartsWith(QuoteRoute.Job.ItemPrefix) => GetJobDisplayName(segment, QuoteRoute.Job.ItemPrefix),
                _ => segment
            };

            breadcrumbs.Add(new FolderPathItem
            {
                Id = current,
                DisplayName = display
            });
        }

        return breadcrumbs;
    }

    public override PathInfo ResolvePathInfo(FfPath path)
    {
        if (path.Segments.Length == 0)
            throw new PluginMisconfigurationException($"The path '{path.Raw}' is not supported.");

        int mainId = ParseId(path.RootSegment.Substring(EntityPrefixes.Quote.Length));
        string[] segments = path.Segments;

        for (int i = 0; i < segments.Length; i++)
        {
            var seg = segments[i];

            if (i == 0 && FolderTypeConstants.Quote.TryGetValue(seg, out var quoteFt))
            {
                return new PathInfo(mainId, quoteFt, string.Join('/', segments.Skip(1)));
            }

            if (seg is QuoteRoute.Items or QuoteRoute.IndependentJobs or QuoteRoute.Item.Jobs)
            {
                continue;
            }

            if (seg.StartsWith(QuoteRoute.Item.Prefix))
            {
                mainId = ParseItemId(seg, QuoteRoute.Item.Prefix);

                if (i + 1 < segments.Length && FolderTypeConstants.QuoteItem.TryGetValue(segments[i + 1], out var itemFt))
                {
                    return new PathInfo(mainId, itemFt, string.Join('/', segments.Skip(i + 2)));
                }
                continue;
            }

            if (seg.StartsWith(QuoteRoute.Job.ItemPrefix) || seg.StartsWith(QuoteRoute.Job.IndependentPrefix))
            {
                string prefix = seg.StartsWith(QuoteRoute.Job.ItemPrefix)
                    ? QuoteRoute.Job.ItemPrefix
                    : QuoteRoute.Job.IndependentPrefix;

                mainId = ParseItemId(seg, prefix);

                if (i + 1 < segments.Length && FolderTypeConstants.QuoteJob.TryGetValue(segments[i + 1], out var jobFt))
                {
                    return new PathInfo(mainId, jobFt, string.Join('/', segments.Skip(i + 2)));
                }

                return new PathInfo(mainId, FolderTypeConstants.QuoteJob["!_In"], string.Join('/', segments.Skip(i)));
            }
        }

        throw new PluginMisconfigurationException($"The path '{path.Raw}' could not be resolved to a valid Plunet folder.");
    }

    private async Task<IEnumerable<FileDataItem>> HandleQuoteItems(FfPath path, int quoteId, CancellationToken ct)
    {
        var segments = path.Segments.Skip(1).ToArray();

        if (segments.Length == 0)
        {
            var items = await FfClientProvider.ItemClient.getAllItemsAsync(FfClientProvider.Uuid, (int)ItemProjectType.Quote, quoteId);

            if (items.data is null || items.data.Length == 0) return [];

            return items.data
                .Where(id => id.HasValue)
                .Select((id, index) => new Folder 
                { 
                    Id = $"{path.Raw}/{QuoteRoute.Item.Prefix}{id}:{index + 1:D3}", 
                    DisplayName = string.Format(DisplayNames.ItemFormat, index + 1),
                    IsSelectable = IsFolderMode 
                });
        }

        if (!segments[0].StartsWith(QuoteRoute.Item.Prefix)) return [];

        int itemId = ParseItemId(segments[0], QuoteRoute.Item.Prefix);

        return await HandleQuoteItem(path, itemId, segments.Skip(1).ToArray(), ct);
    }

    private async Task<IEnumerable<FileDataItem>> HandleQuoteItem(FfPath path, int itemId, string[] segments, CancellationToken ct)
    {
        if (segments.Length == 0)
        {
            return new[]
            {
                new Folder { DisplayName = "Jobs (item dependent)", Id = $"{path.Raw}/{QuoteRoute.Item.Jobs}" }
            }
            .Concat(FolderTypeConstants.QuoteItem.Select(f => new Folder { DisplayName = f.Key, Id = $"{path.Raw}/{f.Key}" }));
        }

        var segment = segments[0];

        if (FolderTypeConstants.QuoteItem.TryGetValue(segment, out var folderType))
        {
            return await ListContentAsync(path.Raw, itemId, folderType, segments.Skip(1).ToArray(), ct);
        }

        if (segment == QuoteRoute.Item.Jobs)
            return await HandleItemDependentJobs(path, itemId, segments.Skip(1).ToArray(), ct);

        return [];
    }

    private async Task<IEnumerable<FileDataItem>> HandleItemDependentJobs(FfPath path, int itemId, string[] segments, CancellationToken ct)
    {
        if (segments.Length == 0)
        {
            var jobs = await FfClientProvider.JobClient.getJobListOfItem_ForViewAsync(FfClientProvider.Uuid, itemId, (int)ItemProjectType.Quote);

            return jobs.data.Length == 0
                ? []
                : jobs.data.Select(job => new Folder
                {
                    Id = $"{path.Raw}/{QuoteRoute.Job.ItemPrefix}{job.jobID}:{job.jobTypeShort}",
                    DisplayName = string.Format(DisplayNames.JobFormat, job.jobTypeShort, job.jobID)
                });
        }

        if (!segments[0].StartsWith(QuoteRoute.Job.ItemPrefix)) return [];

        int jobId = ParseItemId(segments[0], QuoteRoute.Job.ItemPrefix);

        return await HandleQuoteJob(path, jobId, segments.Skip(1).ToArray(), ct);
    }

    private async Task<IEnumerable<FileDataItem>> HandleItemIndependentJobs(FfPath path, int quoteId, CancellationToken ct)
    {
        var segments = path.Segments.Skip(1).ToArray();

        if (segments.Length == 0)
        {
            var jobs = await FfClientProvider.JobClient.getItemIndependentJobsAsync(FfClientProvider.Uuid, (int)ItemProjectType.Quote, quoteId);
            return jobs.data.Length == 0
                ? []
                : jobs.data.Select(job => new Folder
                {
                    Id = $"{path.Raw}/{QuoteRoute.Job.IndependentPrefix}{job.jobID}:{job.jobTypeShort}",
                    DisplayName = string.Format(DisplayNames.JobFormat, job.jobTypeShort, job.jobID)
                });
        }

        if (!segments[0].StartsWith(QuoteRoute.Job.IndependentPrefix)) return [];

        int jobId = ParseItemId(segments[0], QuoteRoute.Job.IndependentPrefix);

        return await HandleQuoteJob(path, jobId, segments.Skip(1).ToArray(), ct);
    }

    private async Task<IEnumerable<FileDataItem>> HandleQuoteJob(FfPath path, int jobId, string[] segments, CancellationToken ct)
    {
        if (segments.Length == 0)
        {
            return FolderTypeConstants.QuoteJob.Select(f => new Folder { DisplayName = f.Key, Id = $"{path.Raw}/{f.Key}" });
        }

        if (FolderTypeConstants.QuoteJob.TryGetValue(segments[0], out var folderType))
        {
            return await ListContentAsync(path.Raw, jobId, folderType, segments.Skip(1).ToArray(), ct);
        }

        return [];
    }
}