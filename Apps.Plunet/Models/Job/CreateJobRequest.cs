using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.Job
{
    public class CreateJobRequest
    {

        [Display("Due date")]
        public DateTime? DueDate { get; set; }

        [Display("Item")]
        public string ItemId { get; set; }

        [Display("Start date")]
        public DateTime? StartDate { get; set; }

        [Display("Status")]
        [DataSource(typeof(JobStatusDataHandler))]
        public string? Status { get; set; }
    }
}
