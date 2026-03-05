using Apps.Plunet.Constants;
using Apps.Plunet.DataOutgoingInvoice30Service;
using Apps.Plunet.Models.Enums;
using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using Blackbird.Plugins.Plunet.DataPayable30Service;
using static Apps.Plunet.Constants.FolderConstants;

namespace Apps.Plunet.Strategies;

public class InvoiceStrategy(FfClientProvider ffClientProvider, PickerMode mode) : BaseStrategy(ffClientProvider, mode), IPlunetStrategy
{
    public override bool CanHandle(FfPath path) => path.RootSegment.StartsWith(EntityPrefixes.Invoice);

    public override async Task<IEnumerable<FileDataItem>> HandleAsync(FfPath path, CancellationToken ct)
    {
        switch (path.Segments[0])
        {
            case FolderTypeConstants.ReceivableInvoiceKey:
            {
                var invoices = await FfClientProvider.OutgoingInvoiceClient.searchAsync(FfClientProvider.Uuid, new SearchFilter_Invoice());
                return MapToVirtualFolders(invoices.data, InvoiceDirections.Receivable);
            }
            case FolderTypeConstants.PayableInvoiceKey:
            {
                var invoices = await FfClientProvider.PayableClient.searchAsync(FfClientProvider.Uuid, new SearchFilter_Payable());
                return MapToVirtualFolders(invoices.data, InvoiceDirections.Payable);
            }
            default:
                return [];
        }
    }

    public override IEnumerable<FolderPathItem> ResolveFolderPath(FfPath path)
    {
        var breadcrumbs = new List<FolderPathItem>
        {
            new() { Id = VirtualRoots.Home, DisplayName = DisplayNames.Home },
            new() { Id = VirtualRoots.Invoice, DisplayName = DisplayNames.Invoice },
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
        throw new PluginMisconfigurationException($"The path '{path.Raw}' is not supported here.");
    }
}