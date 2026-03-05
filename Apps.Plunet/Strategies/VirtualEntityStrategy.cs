using Apps.Plunet.Constants;
using Apps.Plunet.Models.Enums;
using Apps.Plunet.Models.FFPicker;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using Blackbird.Plugins.Plunet.DataQuote30Service;
using Blackbird.Plugins.Plunet.DataRequest30Service;
using Blackbird.Plugins.Plunet.DataResource30Service;
using static Apps.Plunet.Constants.FolderConstants;

namespace Apps.Plunet.Strategies;

public class VirtualEntityStrategy(FfClientProvider ffClientProvider, PickerMode mode) : BaseStrategy(ffClientProvider, mode), IPlunetStrategy
{
    public override bool CanHandle(FfPath path) => path.RootSegment.StartsWith("v:");

    public override async Task<IEnumerable<FileDataItem>> HandleAsync(FfPath path, CancellationToken ct)
    {
        switch (path.RootSegment)
        {
            case VirtualRoots.Order:
                var orders = await FfClientProvider.OrderClient.searchAsync(FfClientProvider.Uuid, new SearchFilter_Order());
                return MapToVirtualFolders(orders.data, EntityPrefixes.Order);

            case VirtualRoots.Quote:
                var quotes = await FfClientProvider.QuoteClient.searchAsync(FfClientProvider.Uuid, new SearchFilter_Quote());
                return MapToVirtualFolders(quotes.data, EntityPrefixes.Quote);

            case VirtualRoots.Request:
                var requests = await FfClientProvider.RequestClient.searchAsync(FfClientProvider.Uuid, new SearchFilter_Request());
                return MapToVirtualFolders(requests.data, EntityPrefixes.Request);

            case VirtualRoots.Invoice:
                return FolderTypeConstants.Invoice.Select(folder => new Folder { DisplayName = folder.Key, Id = $"{EntityPrefixes.Invoice}/{folder.Key}" });

            case VirtualRoots.Resource:
                var resources = await FfClientProvider.ResourceClient.searchAsync(FfClientProvider.Uuid, new SearchFilter_Resource());
                return MapToVirtualFolders(resources.data, EntityPrefixes.Resource);

            case VirtualRoots.Customer:
                var customers = await FfClientProvider.CustomerClient.searchAsync(FfClientProvider.Uuid, new SearchFilter_Customer());
                return MapToVirtualFolders(customers.data, EntityPrefixes.Customer);

            default:
                return [];
        }
    }

    public override IEnumerable<FolderPathItem> ResolveFolderPath(FfPath path)
    {
        return [ new FolderPathItem { Id = VirtualRoots.Home, DisplayName = DisplayNames.Home }];
    }

    public override PathInfo ResolvePathInfo(FfPath path)
    {
        throw new PluginMisconfigurationException($"The path '{path.Raw}' is not supported here.");
    }
}