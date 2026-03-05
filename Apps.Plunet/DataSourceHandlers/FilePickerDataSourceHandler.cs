using Apps.Plunet.DataOutgoingInvoice30Service;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Enums;
using Apps.Plunet.Models.FFPicker;
using Apps.Plunet.Strategies;
using Apps.Plunet.Utils;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.DataDocument30Service;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataJob30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using Blackbird.Plugins.Plunet.DataPayable30Service;
using Blackbird.Plugins.Plunet.DataQuote30Service;
using Blackbird.Plugins.Plunet.DataRequest30Service;
using Blackbird.Plugins.Plunet.DataResource30Service;


namespace Apps.Plunet.DataSourceHandlers
{
    public class FilePickerDataSourceHandler: PlunetInvocable, IAsyncFileDataSourceItemHandler, IPlunetClientProvider
    {
        private readonly List<IPlunetStrategy> _strategies;

        public FilePickerDataSourceHandler(InvocationContext context) : base(context)
        {
            _strategies =
            [
                new RootStrategy(),
                new VirtualEntityStrategy(this, PickerMode.File),
                new CustomerStrategy(this, PickerMode.File),
                new ResourceStrategy(this, PickerMode.File),
                new RequestStrategy(this, PickerMode.File),
                new InvoiceStrategy(this, PickerMode.File),
                new ReceivableInvoiceStrategy(this, PickerMode.File),
                new PayableInvoiceStrategy(this, PickerMode.File),
                new OrderStrategy(this, PickerMode.File),
                new QuoteStrategy(this, PickerMode.File)
            ];
        }

        string IPlunetClientProvider.Uuid => Uuid;
        DataOrder30Client IPlunetClientProvider.OrderClient => OrderClient;
        DataQuote30Client IPlunetClientProvider.QuoteClient => QuoteClient;
        DataRequest30Client IPlunetClientProvider.RequestClient => RequestClient;
        DataResource30Client IPlunetClientProvider.ResourceClient => ResourceClient;
        DataCustomer30Client IPlunetClientProvider.CustomerClient => CustomerClient;
        DataOutgoingInvoice30Client IPlunetClientProvider.OutgoingInvoiceClient => OutgoingInvoiceClient;
        DataPayable30Client IPlunetClientProvider.PayableClient => PayableClient;
        DataItem30Client IPlunetClientProvider.ItemClient => ItemClient;
        DataJob30Client IPlunetClientProvider.JobClient => JobClient;
        DataDocument30Client IPlunetClientProvider.DocumentClient => DocumentClient;

        public async Task<IEnumerable<FileDataItem>> GetFolderContentAsync(FolderContentDataSourceContext context, CancellationToken cancellationToken)
        {
            var path = PathParser.Parse(context.FolderId);

            var strategy = _strategies.FirstOrDefault(h => h.CanHandle(path));
            if (strategy is null) return [];

            return await strategy.HandleAsync(path, cancellationToken);
        }

        public Task<IEnumerable<FolderPathItem>> GetFolderPathAsync(FolderPathDataSourceContext context, CancellationToken cancellationToken)
        {
            var path = PathParser.Parse(context.FileDataItemId);

            var strategy = _strategies.FirstOrDefault(h => h.CanHandle(path));
            return strategy is null 
                ? Task.FromResult<IEnumerable<FolderPathItem>>([]) 
                : Task.FromResult(strategy.ResolveFolderPath(path));
        }
    }
}