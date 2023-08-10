using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Plugins.Plunet.DataSourceHandlers;

namespace Blackbird.Plugins.Plunet.Models.Customer;

public class CustomerRequest
{
    [Display("Customer")]
    [DataSource(typeof(CustomerIdDataHandler))]
    public string CustomerId { get; set; }
}