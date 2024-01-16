using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.CustomProperties
{
    public class PropertyList
    {
        [Display("Property values")]
        public IEnumerable<string>? Values { get; set; }
    }
}
