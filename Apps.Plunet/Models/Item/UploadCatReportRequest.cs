using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Plunet.Models.Item;

public class UploadCatReportRequest
{
    public File File { get; set; }
    
    [Display("Overwrite existing pricelines")]
    public bool OverwriteExistingPricelines { get; set; }
    
    [Display("CAT type")]
    [DataSource(typeof(CatTypeDataHandler))]
    public string CatType { get; set; }
    
    [Display("Project type")]
    [DataSource(typeof(ProjectOrderQuoteTypeDataHandler))]
    public string ProjectType { get; set; }
    
    [Display("Copy results to item")]
    public bool CopyResultsToItem { get; set; }
}