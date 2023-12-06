using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models
{
    public class PricelineRequest
    {
        [Display("Amount per unit")]
        public double? AmountPerUnit { get; set; }

        [Display("Total amount")]
        public double Amount { get; set; }

        [Display("Memo")]
        public string? Memo { get; set; }

        [Display("Time per unit")]
        public double? TimePerUnit { get; set; }

        [Display("Unit price")]
        public double UnitPrice { get; set; }

        // TODO: Tax type
    }
}
