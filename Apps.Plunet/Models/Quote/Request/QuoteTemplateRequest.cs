using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Quote.Request
{
    public class QuoteTemplateRequest
    {
        [Display("Template")]
        [DataSource(typeof(QuoteTemplateDataHandler))]
        public string? TemplateId { get; set; }
    }
}
