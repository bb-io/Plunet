using Apps.Plunet.Models.Job;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Item
{
    public class ItemJobsResponse
    {
        [Display("Jobs")]
        public IEnumerable<JobResponse> Jobs { get; set; }
    }
}
