using Apps.Plunet.Constants;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Enums;
using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using static Apps.Plunet.Constants.FolderConstants;

namespace Apps.Plunet.Strategies;

public class OrderStrategy(IPlunetClientProvider plunet, PickerMode mode) : BaseStrategy(plunet, mode), IPlunetStrategy
{
    public override bool CanHandle(PlunetPath path) => path.RootSegment.StartsWith(EntityPrefixes.Order);

    public override async Task<IEnumerable<FileDataItem>> HandleAsync(PlunetPath path, CancellationToken ct)
    {
        int mainId = ParseId(path.RootSegment.Substring(EntityPrefixes.Order.Length));

        if (path.Segments.Length == 0)
        {
            return new[]
            {
                new Folder { DisplayName = "Items", Id = $"{path.Raw}/{OrderRoute.Items}" },
                new Folder { DisplayName = "Jobs (item independent)", Id = $"{path.Raw}/{OrderRoute.IndependentJobs}" },
            }.Concat(
                FolderTypeConstants.Order.Select(folder => new Folder
                {
                    DisplayName = folder.Key, Id = $"{path.Raw}/{folder.Key}", IsSelectable = IsFolderMode
                })
            );
        }

        if (FolderTypeConstants.Order.TryGetValue(path.Segments[0], out var folderType))
        {
            return await ListContentAsync(path.Raw, mainId, folderType, path.Segments.Skip(1).ToArray(), ct);
        }

        return path.Segments[0] switch
        {
            OrderRoute.Items => await HandleOrderItems(path, mainId, ct),
            OrderRoute.IndependentJobs => await HandleItemIndependentJobs(path, mainId, ct),
            _ => []
        };
    }

    public override IEnumerable<FolderPathItem> ResolveFolderPath(PlunetPath path)
    {
        int mainId = ParseId(path.RootSegment.Substring(EntityPrefixes.Order.Length));

        var breadcrumbs = new List<FolderPathItem>
        {
            new() { Id = VirtualRoots.Home, DisplayName = DisplayNames.Home },
            new() { Id = VirtualRoots.Order, DisplayName = DisplayNames.Order },
            new() { Id = path.RootSegment, DisplayName = $"{DisplayNames.Id}: {mainId}" }
        };

        if (path.Segments.Length == 0) return breadcrumbs;

        string current = path.RootSegment;

        foreach (var segment in path.Segments)
        {
            current = $"{current}/{segment}";

            string display = segment switch
            {
                OrderRoute.Items => "Items",
                OrderRoute.IndependentJobs => "Jobs (item independent)",
                OrderRoute.Item.Jobs => "Jobs (item dependent)",
                _ when segment.StartsWith(OrderRoute.Item.Prefix) => GetItemDisplayName(segment, OrderRoute.Item.Prefix),
                _ when segment.StartsWith(OrderRoute.Job.IndependentPrefix) => GetJobDisplayName(segment, OrderRoute.Job.IndependentPrefix),
                _ when segment.StartsWith(OrderRoute.Job.ItemPrefix) => GetJobDisplayName(segment, OrderRoute.Job.ItemPrefix),
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

    private async Task<IEnumerable<FileDataItem>> HandleOrderItems(PlunetPath path, int orderId, CancellationToken ct)
    {
        var segments = path.Segments.Skip(1).ToArray();

        if (segments.Length == 0)
        {
            var items = await Plunet.ItemClient.getAllItemsAsync(Plunet.Uuid, (int)ItemProjectType.Order, orderId);

            if (items.data is null || items.data.Length == 0) return [];

            return items.data
                .Where(id => id.HasValue)
                .Select((id, index) => new Folder
                {
                    Id = $"{path.Raw}/{OrderRoute.Item.Prefix}{id}:{index + 1:D3}",
                    DisplayName = string.Format(DisplayNames.ItemFormat, index + 1),
                    IsSelectable = IsFolderMode
                });
        }

        if (!segments[0].StartsWith(OrderRoute.Item.Prefix)) return [];
        
        int itemId = ParseItemId(segments[0], OrderRoute.Item.Prefix);

        return await HandleOrderItem(path, itemId, segments.Skip(1).ToArray(), ct);
    }

    private async Task<IEnumerable<FileDataItem>> HandleOrderItem(PlunetPath path, int itemId, string[] segments, CancellationToken ct)
    {
        if (segments.Length == 0)
        {
            return new[]
            {
                new Folder { DisplayName = "Jobs (item dependent)", Id = $"{path.Raw}/{OrderRoute.Item.Jobs}" }
            }
            .Concat(FolderTypeConstants.OrderItem.Select(f => new Folder { DisplayName = f.Key, Id = $"{path.Raw}/{f.Key}" }));
        }

        var segment = segments[0];

        if (FolderTypeConstants.OrderItem.TryGetValue(segment, out var folderType))
        {
            return await ListContentAsync(path.Raw, itemId, folderType, segments.Skip(1).ToArray(), ct);
        }

        if (segment == OrderRoute.Item.Jobs)
            return await HandleItemDependentJobs(path, itemId, segments.Skip(1).ToArray(), ct);

        return [];
    }

    private async Task<IEnumerable<FileDataItem>> HandleItemDependentJobs(PlunetPath path, int itemId, string[] segments, CancellationToken ct)
    {
        if (segments.Length == 0)
        {
            var jobs = await Plunet.JobClient.getJobListOfItem_ForViewAsync(Plunet.Uuid, itemId, (int)ItemProjectType.Order);

            return jobs.data.Length == 0
                ? []
                : jobs.data.Select(job => new Folder
                {
                    Id = $"{path.Raw}/{OrderRoute.Job.ItemPrefix}{job.jobID}:{job.jobTypeShort}", 
                    DisplayName = string.Format(DisplayNames.JobFormat, job.jobTypeShort, job.jobID)
                });
        }

        if (!segments[0].StartsWith(OrderRoute.Job.ItemPrefix)) return [];

        int jobId = ParseItemId(segments[0], OrderRoute.Job.ItemPrefix);

        return await HandleOrderJob(path, jobId, segments.Skip(1).ToArray(), ct);
    }

    private async Task<IEnumerable<FileDataItem>> HandleItemIndependentJobs(PlunetPath path, int orderId, CancellationToken ct)
    {
        var segments = path.Segments.Skip(1).ToArray();

        if (segments.Length == 0)
        {
            var jobs = await Plunet.JobClient.getItemIndependentJobsAsync(Plunet.Uuid, (int)ItemProjectType.Order, orderId);
            return jobs.data.Length == 0
                ? []
                : jobs.data.Select(job => new Folder
                {
                    Id = $"{path.Raw}/{OrderRoute.Job.IndependentPrefix}{job.jobID}:{job.jobTypeShort}", 
                    DisplayName = string.Format(DisplayNames.JobFormat, job.jobTypeShort, job.jobID)
                });
        }

        if (!segments[0].StartsWith(OrderRoute.Job.IndependentPrefix)) return [];

        int jobId = ParseItemId(segments[0], OrderRoute.Job.IndependentPrefix);

        return await HandleOrderJob(path, jobId, segments.Skip(1).ToArray(), ct);
    }

    private async Task<IEnumerable<FileDataItem>> HandleOrderJob(PlunetPath path, int jobId, string[] segments, CancellationToken ct)
    {
        if (segments.Length == 0)
        {
            return FolderTypeConstants.OrderJob.Select(f => new Folder { DisplayName = f.Key, Id = $"{path.Raw}/{f.Key}" });
        }

        if (FolderTypeConstants.OrderJob.TryGetValue(segments[0], out var folderType))
        {
            return await ListContentAsync(path.Raw, jobId, folderType, segments.Skip(1).ToArray(), ct);
        }

        return [];
    }
}