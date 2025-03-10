using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Job
{
    public class JobTypeOptionRequest
    {
        [Display("Job type")]
        [DataSource(typeof(JobTypeOptionDataHandler))]
        public string? JobType { get; set; }
    }
}
