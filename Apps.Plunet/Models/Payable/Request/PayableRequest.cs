using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Payable.Request;

public class PayableRequest
{
    [Display("Payable ID"), DataSource(typeof(PayableDataSourceHandler))]
    public string PayableId { get; set; } = string.Empty;
}