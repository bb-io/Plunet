using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Job
{
    public class JobFeedbackResponse
    {

        [Display("Job ID")]
        public string JobID { get; set; }

        [Display("Editor user ID")]
        public int EditorUserID { get; set; }

        [Display("Modified at")]
        public DateTime ModifiedAt { get; set; }

        public double Rating { get; set; }

        [Display("Is job quality rating closed")]
        public bool IsJobQualityRatingClosed { get; set; }

        public string? Commentary { get; set; }

    }
}
