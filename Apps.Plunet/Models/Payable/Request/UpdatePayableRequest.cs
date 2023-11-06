using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Plugins.Plunet.DataSourceHandlers.EnumHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.Payable.Request
{
    public class UpdatePayableRequest
    {

        [Display("Payable ID")]
        public string Id { get; set; }

        [Display("Status")]
        [DataSource(typeof(PayableStatusDataHandler))]
        public string? Status { get; set; }

        [Display("External invoice number")]
        public string? ExternalInvoiceNumber { get; set; }

        [Display("Account statement")]
        public string? AccountStatement { get; set; }

        [Display("Creditor account")]
        public string? CreditorAccount { get; set; }

        [Display("Is exported")]
        public bool? IsExported { get; set; }

        [Display("Memo")]
        public string? Memo { get; set; }

        [Display("Invoice date")]
        public DateTime? InvoiceDate { get; set; }

        [Display("Paid date")]
        public DateTime? PaidDate { get; set; }

        [Display("Due date")]
        public DateTime? DueDate { get; set; }

        [Display("Value date")]
        public DateTime? ValueDate { get; set; }
    }
}
