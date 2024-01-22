using Blackbird.Applications.Sdk.Common;
using Blackbird.Plugins.Plunet.DataCustomer30Service;

namespace Apps.Plunet.Models.Customer;

public class GetCustomerResponse
{
    [Display("Academic title")]
    public string AcademicTitle { get; set; }

    [Display("Cost center")]
    public string CostCenter { get; set; }

    [Display("Currency")]
    public string Currency { get; set; }

    [Display("Customer ID")]
    public string CustomerId { get; set; }

    [Display("Email")]
    public string Email { get; set; }

    [Display("External ID")]
    public string ExternalId { get; set; }

    [Display("Fax")]
    public string Fax { get; set; }

    [Display("Full name")]
    public string FullName { get; set; }

    [Display("Mobile phone")]
    public string MobilePhone { get; set; }

    [Display("Name")]
    public string Name { get; set; }

    [Display("Phone")]
    public string Phone { get; set; }

    [Display("Skype ID")]
    public string SkypeId { get; set; }

    [Display("Status")]
    public string Status { get; set; }

    [Display("User ID")]
    public string UserId { get; set; }

    [Display("Website")]
    public string Website { get; set; }

    [Display("Account manager ID")]
    public string? AccountManagerId { get; set; }

    [Display("Payment information")]
    public GetPaymentInfoResponse PaymentInformation { get; set; }

    public GetCustomerResponse(Blackbird.Plugins.Plunet.DataCustomer30Service.Customer customer,
        PaymentInfo paymentInfo, int? accountManagerId = default)
    {
        AcademicTitle = customer.academicTitle;
        CostCenter = customer.costCenter;
        Currency = customer.currency;
        CustomerId = customer.customerID.ToString();
        Email = customer.email;
        ExternalId = customer.externalID;
        Fax = customer.fax;
        FullName = customer.fullName;
        MobilePhone = customer.mobilePhone;
        Name = customer.name1 ?? customer.name2;
        Phone = customer.phone;
        SkypeId = customer.skypeID;
        Status = customer.status.ToString();
        UserId = customer.userId.ToString();
        Website = customer.website;
        AccountManagerId = accountManagerId == 0 ? null : accountManagerId?.ToString();
        PaymentInformation = new GetPaymentInfoResponse(paymentInfo);
    }
}