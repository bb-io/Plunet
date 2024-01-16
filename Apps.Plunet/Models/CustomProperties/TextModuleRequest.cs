using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.CustomProperties
{
    public class TextModuleRequest
    {
        [Display("Entity type")]
        [DataSource(typeof (TextModuleUsageDataHandler))]
        public string UsageArea {  get; set; }

        [Display("Entity ID", Description = "The ID of the entity, e.g. the order ID or the customer ID")]
        public string MainId { get; set; }

        [Display("Flag", Description = "e.g. [Textmodule_1]")]
        public string Flag { get; set; }
    }
}
