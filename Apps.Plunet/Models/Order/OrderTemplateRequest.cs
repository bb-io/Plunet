using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.Order
{
    public class OrderTemplateRequest
    {
        [Display("Template")]
        [DataSource(typeof(TemplateDataHandler))]
        public string TemplateId { get; set; }
    }
}
