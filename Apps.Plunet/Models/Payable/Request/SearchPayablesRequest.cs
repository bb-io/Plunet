using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Payable.Request;

public class SearchPayablesRequest
{
    [Display("Date from")]
    public DateTime DateFrom { get; set; }

    [Display("Date to")]
    public DateTime DateTo { get; set; }

    [Display("Date refers to")]
    [DataSource(typeof(TimeFrameRelationDataHandler))]
    public string TimeFrameRelation { get; set; }


    [DataSource(typeof(ExportedTypeDataHandler))]
    public string? Exported { get; set; }
    
    [DataSource(typeof(PayableStatusDataHandler))]
    public string? Status { get; set; }

    [DataSource(typeof(CurrencyDataSourceHandler))]
    public string? Currency { get; set; }
    
    [Display("Resource ID")]
    public string? ResourceId { get; set; }


    

}