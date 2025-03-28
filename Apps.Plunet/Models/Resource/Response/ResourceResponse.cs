﻿using Blackbird.Applications.Sdk.Common;

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

    [Display("Last name")] public string Name1 { get; set; }

    [Display("First name")] public string Name2 { get; set; }

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

    [Display("Payment info")] public ResourcePaymentResponse Payment { get; set; }
    
    [Display("Delivery address")]
    public AddressResponse DeliveryAddress { get; set; }

    [Display("Invoice address")]
    public AddressResponse InvoiceAddress { get; set; }

    public ResourceResponse(Blackbird.Plugins.Plunet.DataResource30Service.Resource resource, Blackbird.Plugins.Plunet.DataResource30Service.PaymentInfo paymentInfo, AddressResponse delivery, AddressResponse invoice)
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
        Name1 = resource.name1;
        Name2 = resource.name2;
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
        DeliveryAddress = delivery;
        InvoiceAddress = invoice;
    }
}

public class AddressResponse
{
    [Display("Name 1")]
    public string? Name1 { get; set; }

    [Display("Name 2")]
    public string? Name2 { get; set; }

    [Display("Description")]
    public string? Description { get; set; }

    [Display("Country")]
    public string? Country { get; set; }

    [Display("State")]
    public string? State { get; set; }

    [Display("City")]
    public string? City { get; set; }

    [Display("Street")]
    public string? Street { get; set; }

    [Display("Street 2")]
    public string? Street2 { get; set; }

    [Display("ZIP code")]
    public string? ZipCode { get; set; }

    [Display("Office")]
    public string? Office { get; set; }

}