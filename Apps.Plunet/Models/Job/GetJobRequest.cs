using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Models.Job;

public class GetJobRequest
{
    [Display("Project type")]
    [StaticDataSource(typeof(ItemProjectTypeDataHandler))]
    public string ProjectType { get; set; }

    [Display("Job ID")]
    public string JobId { get; set; }
}
