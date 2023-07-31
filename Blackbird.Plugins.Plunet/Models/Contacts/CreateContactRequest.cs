using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Contacts;

public class CreateContactRequest
{
    [Display("Customer ID")] public string CustomerId { get; set; }

    [Display("First name")] public string? FirstName { get; set; }

    [Display("Last name")] public string? LastName { get; set; }

    [Display("Email")] public string? Email { get; set; }

    [Display("Mobile phone")] public string? MobilePhone { get; set; }

    [Display("Telephone number")] public string? Phone { get; set; }

    [Display("User ID")] public string? UserId { get; set; }

    [Display("Supervisor 1")] public string? Supervisor1 { get; set; }

    [Display("Supervisor 2")] public string? Supervisor2 { get; set; }

    [Display("External ID")] public string? ExternalId { get; set; }

    [Display("Address ID")] public string? AddressId { get; set; }
    
    [Display("Cost center")] public string CostCenter { get; set; }
    
    public string Fax { get; set; }
}