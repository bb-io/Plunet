using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Webhooks.Models.Parameters
{
    public class CustomerStatusParameter
    {
        [Display("New status")]
        [DataSource(typeof(CustomerStatusDataHandler))]
        public string? Status { get; set; }
    }
}
