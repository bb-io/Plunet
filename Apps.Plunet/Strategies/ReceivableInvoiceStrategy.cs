using Apps.Plunet.Constants;
using Apps.Plunet.Models.Enums;
using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using static Apps.Plunet.Constants.FolderConstants;

namespace Apps.Plunet.Strategies;

public class ReceivableInvoiceStrategy(FfClientProvider ffClientProvider, PickerMode mode) : BaseStrategy(ffClientProvider, mode), IPlunetStrategy
{
    public override bool CanHandle(FfPath path) => path.RootSegment.StartsWith(InvoiceDirections.Receivable);

    public override async Task<IEnumerable<FileDataItem>> HandleAsync(FfPath path, CancellationToken ct)
    {
        int mainId = ParseId(path.RootSegment.Substring(InvoiceDirections.Receivable.Length));
        return await ListContentAsync(path.Raw, mainId, FolderTypeConstants.Invoice[FolderTypeConstants.ReceivableInvoiceKey], path.Segments, ct);
    }

    public override IEnumerable<FolderPathItem> ResolveFolderPath(FfPath path)
    {
        int mainId = ParseId(path.RootSegment.Substring(InvoiceDirections.Receivable.Length));

        var breadcrumbs = new List<FolderPathItem>
        {
            new() { Id = VirtualRoots.Home, DisplayName = DisplayNames.Home },
            new() { Id = VirtualRoots.Invoice, DisplayName = DisplayNames.Invoice },
            new() { Id = $"{EntityPrefixes.Invoice}/{InvoiceDirections.Receivable}", DisplayName = FolderTypeConstants.ReceivableInvoiceKey },
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
        int mainId = ParseId(path.RootSegment.Substring(InvoiceDirections.Receivable.Length));

        return new PathInfo(
            mainId,
            FolderTypeConstants.Invoice[FolderTypeConstants.ReceivableInvoiceKey],
            string.Join('/', path.Segments)
        );
    }
}