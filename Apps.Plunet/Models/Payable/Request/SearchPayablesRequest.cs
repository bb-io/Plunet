using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Payable.Request;

public class SearchPayablesRequest
{
    [Display("Date from")]
    public DateTime DateFrom { get; set; }

    [Display("Date to")]
    public DateTime DateTo { get; set; }

    [Display("Date refers to")]
    [StaticDataSource(typeof(TimeFrameRelationDataHandler))]
    public string TimeFrameRelation { get; set; }


    [StaticDataSource(typeof(ExportedTypeDataHandler))]
    public string? Exported { get; set; }
    
    [StaticDataSource(typeof(PayableStatusDataHandler))]
    public string? Status { get; set; }

    [DataSource(typeof(CurrencyDataSourceHandler))]
    public string? Currency { get; set; }
    
    [Display("Resource ID")]
    public string? ResourceId { get; set; }


    

}