using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.Models.Item;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Order;

public class LinkOrdersRequest : ProjectTypeRequest
{
    [DataSource(typeof(OrderIdDataHandler))]
    [Display("Source order ID")]
    public string SourceOrderId { get; set; }

    [Display("Target ID")]
    public string TargetId { get; set; }

    [Display("Is bidirectional?")]
    public bool IsBidirectional { get; set; }
    public string Memo { get; set; }
}
