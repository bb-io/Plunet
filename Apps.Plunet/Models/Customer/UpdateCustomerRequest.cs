using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Customer;

public class UpdateCustomerRequest : CreateCustomerRequest
{
    [Display("Customer")]
    [DataSource(typeof(CustomerIdDataHandler))]
    public string CustomerId { get; set; }
}