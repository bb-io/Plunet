using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Order
{
    public class OrderTemplateRequest
    {
        [Display("Template")]
        [DataSource(typeof(TemplateDataHandler))]
        public string? TemplateId { get; set; }
    }
}
