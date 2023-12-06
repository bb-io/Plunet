using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models
{
    public class PricelinesResponse
    {
        [Display("Pricelines")]
        public IEnumerable<PricelineResponse> Pricelines { get; set; }
    }
}
