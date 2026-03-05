using Apps.Plunet.Constants;
using Apps.Plunet.Models.Enums;
using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using Blackbird.Plugins.Plunet.DataQuote30Service;
using Blackbird.Plugins.Plunet.DataRequest30Service;
using Blackbird.Plugins.Plunet.DataResource30Service;
using static Apps.Plunet.Constants.FolderConstants;

namespace Apps.Plunet.Strategies;

public class VirtualEntityStrategy(IPlunetClientProvider plunet, PickerMode mode) : BaseStrategy(plunet, mode), IPlunetStrategy
{
    public override bool CanHandle(PlunetPath path) => path.RootSegment.StartsWith("v:");

    public override async Task<IEnumerable<FileDataItem>> HandleAsync(PlunetPath path, CancellationToken ct)
    {
        switch (path.RootSegment)
        {
            case VirtualRoots.Order:
                var orders = await Plunet.OrderClient.searchAsync(Plunet.Uuid, new SearchFilter_Order());
                return MapToVirtualFolders(orders.data, EntityPrefixes.Order);

            case VirtualRoots.Quote:
                var quotes = await Plunet.QuoteClient.searchAsync(Plunet.Uuid, new SearchFilter_Quote());
                return MapToVirtualFolders(quotes.data, EntityPrefixes.Quote);

            case VirtualRoots.Request:
                var requests = await Plunet.RequestClient.searchAsync(Plunet.Uuid, new SearchFilter_Request());
                return MapToVirtualFolders(requests.data, EntityPrefixes.Request);

            case VirtualRoots.Invoice:
                return FolderTypeConstants.Invoice.Select(folder => new Folder { DisplayName = folder.Key, Id = $"{EntityPrefixes.Invoice}/{folder.Key}" });

            case VirtualRoots.Resource:
                var resources = await Plunet.ResourceClient.searchAsync(Plunet.Uuid, new SearchFilter_Resource());
                return MapToVirtualFolders(resources.data, EntityPrefixes.Resource);

            case VirtualRoots.Customer:
                var customers = await Plunet.CustomerClient.searchAsync(Plunet.Uuid, new SearchFilter_Customer());
                return MapToVirtualFolders(customers.data, EntityPrefixes.Customer);

            default:
                return [];
        }
    }

    public override IEnumerable<FolderPathItem> ResolveFolderPath(PlunetPath path)
    {
        return [ new FolderPathItem { Id = VirtualRoots.Home, DisplayName = DisplayNames.Home }];
    }
}