using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Job
{
    public class JobRoundDto
    {
        [Display("Job round ID")]
        public string Id { get; set; }

        [Display("Job round number")]
        public int Number { get; set; }

        [Display("Assignment limit to first X")]
        public string assignmentLimitToFirstX { get; set; }

        [Display("Assignment limit type")]
        public string assignmentLimitType { get; set; }

        [Display("Assignment mehod")]
        public string assignmentMethod { get; set; }

        public bool Assigned { get; set; }
    }
}
