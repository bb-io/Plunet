using Blackbird.Applications.Sdk.Common;
using Blackbird.Plugins.Plunet.DataCustomerContact30Service;

namespace Blackbird.Plugins.Plunet.Models.Contacts;

public class ContactObjectResponse
{
    [Display("Cost center")] public string CostCenter { get; set; }

    [Display("Customer contact ID")] public string CustomerContactId { get; set; }

    [Display("Customer ID")] public string CustomerId { get; set; }

    [Display("Email")] public string Email { get; set; }

    [Display("External ID")] public string ExternalId { get; set; }

    [Display("Fax")] public string Fax { get; set; }
    [Display("First name")] public string FirstName { get; set; }
    [Display("Last name")] public string LastName { get; set; }
    [Display("Mobile phone")] public string MobilePhone { get; set; }

    [Display("Phone")] public string Phone { get; set; }

    [Display("Status")] public int Status { get; set; }

    [Display("User ID")] public string UserId { get; set; }

    [Display("Supervisor")] public string Supervisor { get; set; }

    public ContactObjectResponse(CustomerContact customerContact)
    {
        CostCenter = customerContact.costCenter;
        CustomerContactId = customerContact.customerContactID.ToString();
        CustomerId = customerContact.customerID.ToString();
        Email = customerContact.email;
        ExternalId = customerContact.externalID;
        Fax = customerContact.fax;
        FirstName = customerContact.name1;
        LastName = customerContact.name2;
        MobilePhone = customerContact.mobilePhone;
        Phone = customerContact.phone;
        UserId = customerContact.userId.ToString();
        Status = customerContact.status;
        Supervisor = customerContact.supervisor1;
    }
}