using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Quote.Request
{
    public class GetQuoteRequest
    {
        [Display("Quote ID")]
        [DataSource(typeof(QuoteIdDataHandler))]
        public string QuoteId { get; set; }
    }
}
