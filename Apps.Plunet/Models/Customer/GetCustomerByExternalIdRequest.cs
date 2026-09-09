using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Customer;

public class GetCustomerByExternalIdRequest
{
    [Display("External ID")]
    public string ExternalId { get; set; } = string.Empty;
}
