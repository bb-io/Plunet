using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.Item
{
    public class OptionalJobStatusRequest
    {
        [Display("Status")]
        [DataSource(typeof(JobStatusDataHandler))]
        public string? Status { get; set; }
    }
}
