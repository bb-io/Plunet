using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Plugins.Plunet.DataSourceHandlers.EnumHandlers;

namespace Blackbird.Plugins.Plunet.Models.Payable.Request;

public class SearchPayablesRequest
{
    [DataSource(typeof(ExportedTypeDataHandler))]
    public string? Exported { get; set; }
    
    [DataSource(typeof(PayableStatusDataHandler))]
    public string? Status { get; set; }
    
    [DataSource(typeof(TimeFrameRelationDataHandler))]
    public string TimeFrameRelation { get; set; }
    
    [Display("Language code")]
    public string? LanguageCode { get; set; }
    
    [Display("Resource ID")]
    public string? ResourceId { get; set; }
    
    [Display("Date to")]
    public DateTime? DateTo { get; set; }
    
    [Display("Date from")]
    public DateTime? DateFrom { get; set; }
}