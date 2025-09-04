using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Resource.Response
{
    public class ResourcePaymentResponse
    {
        [Display("Account holder")]
        public string AccountHolder { get; set; }

        [Display("Account ID")]
        public string AccountId { get; set; }

        [Display("Bank code")]
        public string Bic { get; set; }

        [Display("Contract number")]
        public string ContractNumber { get; set; }

        [Display("Debit account")]
        public string DebitAccount { get; set; }

        [Display("IBAN")]
        public string Iban { get; set; }

        [Display("Payment method ID")]
        public string PaymentMethodId { get; set; }

        [Display("Preselected tax ID")]
        public string PreselectdTaxId { get; set; }

        [Display("Sales tax ID")]
        public string SalesTaxId { get; set; }
    }
}
