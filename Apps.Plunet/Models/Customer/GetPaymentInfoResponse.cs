using Blackbird.Applications.Sdk.Common;
using Blackbird.Plugins.Plunet.DataCustomer30Service;

namespace Blackbird.Plugins.Plunet.Models.Customer;

public class GetPaymentInfoResponse
{
    [Display("Account holder")] public string AccountHolder { get; set; }

    [Display("Account ID")] public string AccountId { get; set; }

    [Display("BIC")] public string Bic { get; set; }

    [Display("Contract number")] public string ContractNumber { get; set; }

    [Display("Debit ccount")] public string DebitAccount { get; set; }

    [Display("IBAN")] public string Iban { get; set; }

    [Display("Payment method ID")] public string PaymentMethodId { get; set; }

    [Display("Preselected tax ID")] public string PreselectedTaxId { get; set; }

    [Display("Sales tax ID")] public string SalesTaxId { get; set; }

    public GetPaymentInfoResponse(PaymentInfo paymentInfo)
    {
        AccountHolder = paymentInfo.accountHolder;
        AccountId = paymentInfo.accountID.ToString();
        Bic = paymentInfo.BIC;
        ContractNumber = paymentInfo.contractNumber;
        DebitAccount = paymentInfo.debitAccount;
        Iban = paymentInfo.IBAN;
        PaymentMethodId = paymentInfo.paymentMethodID.ToString();
        PreselectedTaxId = paymentInfo.preselectedTaxID.ToString();
        SalesTaxId = paymentInfo.salesTaxID;
    }
}