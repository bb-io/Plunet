﻿using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Customer;

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
    public int Status { get; set; }

    [Display("User ID")]
    public string UserId { get; set; }

    [Display("Website")]
    public string Website { get; set; }

    public GetCustomerResponse(DataCustomer30Service.Customer customer)
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
        Status = customer.status;
        UserId = customer.userId.ToString();
        Website = customer.website;
    }
}