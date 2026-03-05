using Apps.Plunet.Constants;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Enums;
using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using static Apps.Plunet.Constants.FolderConstants;

namespace Apps.Plunet.Strategies;

public class QuoteStrategy(IPlunetClientProvider plunet, PickerMode mode) : BaseStrategy(plunet, mode), IPlunetStrategy
{
    public override bool CanHandle(PlunetPath path) => path.RootSegment.StartsWith(EntityPrefixes.Quote);

    public override async Task<IEnumerable<FileDataItem>> HandleAsync(PlunetPath path, CancellationToken ct)
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

    public override IEnumerable<FolderPathItem> ResolveFolderPath(PlunetPath path)
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

    private async Task<IEnumerable<FileDataItem>> HandleQuoteItems(PlunetPath path, int quoteId, CancellationToken ct)
    {
        var segments = path.Segments.Skip(1).ToArray();

        if (segments.Length == 0)
        {
            var items = await Plunet.ItemClient.getAllItemsAsync(Plunet.Uuid, (int)ItemProjectType.Quote, quoteId);

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

    private async Task<IEnumerable<FileDataItem>> HandleQuoteItem(PlunetPath path, int itemId, string[] segments, CancellationToken ct)
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

    private async Task<IEnumerable<FileDataItem>> HandleItemDependentJobs(PlunetPath path, int itemId, string[] segments, CancellationToken ct)
    {
        if (segments.Length == 0)
        {
            var jobs = await Plunet.JobClient.getJobListOfItem_ForViewAsync(Plunet.Uuid, itemId, (int)ItemProjectType.Quote);

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

    private async Task<IEnumerable<FileDataItem>> HandleItemIndependentJobs(PlunetPath path, int quoteId, CancellationToken ct)
    {
        var segments = path.Segments.Skip(1).ToArray();

        if (segments.Length == 0)
        {
            var jobs = await Plunet.JobClient.getItemIndependentJobsAsync(Plunet.Uuid, (int)ItemProjectType.Quote, quoteId);
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

    private async Task<IEnumerable<FileDataItem>> HandleQuoteJob(PlunetPath path, int jobId, string[] segments, CancellationToken ct)
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