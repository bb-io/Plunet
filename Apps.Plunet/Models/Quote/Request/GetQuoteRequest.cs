using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.Quote.Request
{
    public class GetQuoteRequest
    {
        [Display("Quote ID")]
        public string QuoteId { get; set; }
    }
}
