using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.Item
{
    public class ProjectIdRequest
    {
        [Display("Order or quote ID")]
        [DataSource(typeof(ProjectDataSourceHandler))]
        public string ProjectId { get; set; }
    }
}
