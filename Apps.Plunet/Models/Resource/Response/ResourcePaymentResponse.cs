using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.Resource.Response
{
    public class ResourcePaymentResponse
    {
        [Display("Account holder")]
        public string AccountHolder { get; set; }

        [Display("Account ID")]
        public string AccountId { get; set; }

        [Display("SWIFT - BIC")]
        public string Bic { get; set; }

        [Display("Contract number")]
        public string ContractNumber { get; set; }

        [Display("Debit account")]
        public string DebitAccount { get; set; }

        [Display("IBAN")]
        public string Iban { get; set; }

        [Display("Payment method")]
        public string PaymentMethodId { get; set; }

        [Display("Preselected tax")]
        public string PreselectdTaxId { get; set; }

        [Display("Sales tax")]
        public string SalesTaxId { get; set; }
    }
}
