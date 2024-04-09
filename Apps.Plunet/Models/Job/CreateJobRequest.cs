using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Job
{
    public class CreateJobRequest
    {

        [Display("Due date")]
        public DateTime? DueDate { get; set; }

        [Display("Item ID")]
        public string ItemId { get; set; }

        [Display("Project ID")]
        public string ProjectId { get; set; }

        [Display("Start date")]
        public DateTime? StartDate { get; set; }

        [Display("Status")]
        [StaticDataSource(typeof(JobStatusDataHandler))]
        public string? Status { get; set; }
    }
}
