using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Job
{
    public class JobRoundsResponse
    {
        [Display("Job rounds")]
        public List<JobRoundDto>? JobRounds {get; set;}
    }
}
