using Apps.Plunet.DataOutgoingInvoice30Service;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.DataDocument30Service;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataJob30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using Blackbird.Plugins.Plunet.DataPayable30Service;
using Blackbird.Plugins.Plunet.DataQuote30Service;
using Blackbird.Plugins.Plunet.DataRequest30Service;
using Blackbird.Plugins.Plunet.DataResource30Service;

namespace Apps.Plunet.Models.FFPicker;

public class FfClientProvider(
    string uuid,
    DataOrder30Client orderClient,
    DataQuote30Client quoteClient,
    DataRequest30Client requestClient,
    DataResource30Client resourceClient,
    DataCustomer30Client customerClient,
    DataOutgoingInvoice30Client outgoingInvoiceClient,
    DataPayable30Client payableClient,
    DataItem30Client itemClient,
    DataJob30Client jobClient,
    DataDocument30Client documentClient
)
{
    public string Uuid { get; } = uuid;
    public DataOrder30Client OrderClient { get; } = orderClient;
    public DataQuote30Client QuoteClient { get; } = quoteClient;
    public DataRequest30Client RequestClient { get; } = requestClient;
    public DataResource30Client ResourceClient { get; } = resourceClient;
    public DataCustomer30Client CustomerClient { get; } = customerClient;
    public DataOutgoingInvoice30Client OutgoingInvoiceClient { get; } = outgoingInvoiceClient;
    public DataPayable30Client PayableClient { get; } = payableClient;
    public DataItem30Client ItemClient { get; } = itemClient;
    public DataJob30Client JobClient { get; } = jobClient;
    public DataDocument30Client DocumentClient { get; } = documentClient;
}