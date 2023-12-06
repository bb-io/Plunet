using Apps.Plunet.Models.Job;
using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.Item
{
    public class ItemJobsResponse
    {
        [Display("Jobs")]
        public IEnumerable<JobResponse> Jobs { get; set; }
    }
}
