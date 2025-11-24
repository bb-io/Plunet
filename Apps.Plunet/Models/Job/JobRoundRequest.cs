using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Job;

public class JobRoundRequest
{
    [Display("Job round ID")]
    [DataSource(typeof(JobRoundDataHandler))]
    public string JobRoundId { get; set; }
}
