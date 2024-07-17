using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.CustomProperties
{
    public class MultipleTextModuleResponse
    {
        [Display("Text module values")]
        public IEnumerable<string> Values { get; set; }
    }
}
