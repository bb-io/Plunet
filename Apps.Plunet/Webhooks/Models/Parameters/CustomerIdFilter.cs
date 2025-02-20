using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Webhooks.Models.Parameters
{
    public class CustomerIdFilter
    {
        [Display("Customer ID")]
        [DataSource(typeof(CustomerIdDataHandler))]
        public string? CustomerId { get; set; }
    }
}
