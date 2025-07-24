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

        [Display("Sub scores")]
        public IEnumerable<qualityScore>? SubScores { get; set; }

    }

    public class qualityScore
    {
        [Display("Criterion name")]
        public string Name { get; set; }

        [Display("Critical amount")]
        public double Critical { get; set; }

        [Display("Hard amount")]
        public double Hard { get; set; }

        [Display("Minor amount")]
        public double Minor { get; set; }

        [Display("Criterion rating")]
        public double Rating { get; set; }
    }
}
