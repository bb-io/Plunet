using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Request.Request;

public class SelectJobRoundResourceRequest
{
    [Display("Number of resources to select")]
    public int NumberOfResources { get; set; }

    [Display("Resource IDs to exclude")]
    [DataSource(typeof(RoundResourceDataHandler))]
    public List<string>? ResourceIdsToExclude { get; set; }
}
