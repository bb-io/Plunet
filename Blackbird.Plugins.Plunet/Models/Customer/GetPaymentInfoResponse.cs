namespace Blackbird.Plugins.Plunet.Models.Customer;

public class GetPaymentInfoResponse
{
    public string AccountHolder { get; set; }
        
    public int AccountId { get; set; }
        
    public string BIC { get; set; }
        
    public string ContractNumber { get; set; }
        
    public string DebitAccount { get; set; }
        
    public string IBAN { get; set; }
        
    public int PaymentMethodId { get; set; }
        
    public int PreselectedTaxId { get; set; }
        
    public string SalesTaxId { get; set; }
        
}