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
    public class GetJobRequest
    {
        [Display("Type")]
        [DataSource(typeof(ItemProjectTypeDataHandler))]
        public string ProjectType { get; set; }

        [Display("Job ID")]
        public string JobId { get; set; }
    }
}
