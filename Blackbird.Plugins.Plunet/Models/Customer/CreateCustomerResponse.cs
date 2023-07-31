using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Customer;

public class CreateCustomerResponse
{
    [Display("Customer ID")]
    public string CustomerId { get; set; }
}