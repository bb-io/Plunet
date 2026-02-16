using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;


namespace Apps.Plunet.Models.Item;

public class UploadCatReportToJobRequest
{
    public FileReference File { get; set; }
    
    [Display("Overwrite existing pricelines")]
    public bool OverwriteExistingPricelines { get; set; }
    
    [Display("CAT type")]
    [StaticDataSource(typeof(CatTypeDataHandler))]
    public string CatType { get; set; }
        
    [Display("Copy results to job")]
    public bool CopyResultsToJob { get; set; }
}