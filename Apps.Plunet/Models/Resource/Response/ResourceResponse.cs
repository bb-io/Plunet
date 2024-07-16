using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Resource.Response;

public class ResourceResponse
{
    [Display("Academic title")] public string AcademicTitle { get; set; }

    [Display("Cost center")] public string CostCenter { get; set; }

    [Display("Currency")] public string Currency { get; set; }

    [Display("Email")] public string Email { get; set; }

    [Display("External ID")] public string ExternalID { get; set; }

    [Display("Fax")] public string Fax { get; set; }

    [Display("Form of address")] public string FormOfAddress { get; set; }

    [Display("Full name")] public string FullName { get; set; }

    [Display("Mobile phone")] public string MobilePhone { get; set; }

    [Display("Last name")] public string LastName { get; set; }

    [Display("First name")] public string FirstName { get; set; }

    [Display("Opening")] public string Opening { get; set; }

    [Display("Phone")] public string Phone { get; set; }

    [Display("Resource ID")] public string ResourceID { get; set; }

    [Display("Resource type")] public string ResourceType { get; set; }

    [Display("Skype ID")] public string SkypeID { get; set; }

    [Display("Status")] public string Status { get; set; }

    [Display("Supervisor 1")] public string Supervisor1 { get; set; }

    [Display("Supervisor 2")] public string Supervisor2 { get; set; }

    [Display("User ID")] public string UserID { get; set; }

    [Display("Website")] public string Website { get; set; }

    [Display("Working status")] public string WorkingStatus { get; set; }

    [Display("Payment")] public ResourcePaymentResponse Payment { get; set; }

    public ResourceResponse(Blackbird.Plugins.Plunet.DataResource30Service.Resource resource, Blackbird.Plugins.Plunet.DataResource30Service.PaymentInfo paymentInfo)
    {
        AcademicTitle = resource.academicTitle;
        CostCenter = resource.costCenter;
        Currency = resource.currency;
        Email = resource.email;
        ExternalID = resource.externalID;
        Fax = resource.fax;
        FormOfAddress = resource.formOfAddress.ToString();
        FullName = resource.fullName;
        MobilePhone = resource.mobilePhone;
        LastName = resource.name1;
        FirstName = resource.name2;
        Opening = resource.opening;
        Phone = resource.phone;
        ResourceID = resource.resourceID.ToString();
        ResourceType = resource.resourceType.ToString();
        SkypeID = resource.skypeID;
        Status = resource.status.ToString();
        Supervisor1 = resource.supervisor1;
        Supervisor2 = resource.supervisor2;
        UserID = resource.userId.ToString();
        Website = resource.website;
        WorkingStatus = resource.workingStatus.ToString();
        Payment = new ResourcePaymentResponse
        {
            AccountHolder = paymentInfo.accountHolder,
            AccountId = paymentInfo.accountID.ToString(),
            Bic = paymentInfo.BIC,
            ContractNumber = paymentInfo.contractNumber,
            DebitAccount = paymentInfo.debitAccount,
            Iban = paymentInfo.IBAN,
            PaymentMethodId = paymentInfo.paymentMethodID.ToString(),
            PreselectdTaxId = paymentInfo.preselectedTaxID.ToString(),
            SalesTaxId = paymentInfo.salesTaxID,
        };

    }
}