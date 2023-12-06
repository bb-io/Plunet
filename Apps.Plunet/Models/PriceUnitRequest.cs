using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models
{
    public class PriceUnitRequest
    {
        [Display("Service")]
        [DataSource(typeof(ServiceNameDataHandler))]
        public string Service { get; set; }

        [Display("Price unit")]
        public string PriceUnit { get; set; }
    }
}
