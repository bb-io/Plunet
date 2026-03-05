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

public interface IPlunetClientProvider
{
    string Uuid { get; }

    DataOrder30Client OrderClient { get; }

    DataQuote30Client QuoteClient { get; }

    DataRequest30Client RequestClient { get; }

    DataResource30Client ResourceClient { get; }

    DataCustomer30Client CustomerClient { get; }

    DataOutgoingInvoice30Client OutgoingInvoiceClient { get; }

    DataPayable30Client PayableClient { get; }

    DataItem30Client ItemClient { get; }

    DataJob30Client JobClient { get; }

    DataDocument30Client DocumentClient { get; }
}