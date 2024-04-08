using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Models.CustomProperties
{
    public class SetPropertyRequest
    {
        [Display("Entity type")]
        [StaticDataSource(typeof(PropertyUsageDataHandler))]
        public string UsageArea { get; set; }

        [Display("Entity ID", Description = "The ID of the entity, e.g. the order ID or the customer ID")]
        public string MainId { get; set; }

        [Display("Name", Description = "From Admin -> Properties, in English")]
        public string Name { get; set; }

        [Display("Property values")]
        public IEnumerable<string> Values { get; set; }
    }
}
