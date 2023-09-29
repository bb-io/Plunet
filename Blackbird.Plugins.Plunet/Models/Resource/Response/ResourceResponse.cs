using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Resource.Response;

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

    [Display("Name 1")] public string Name1 { get; set; }

    [Display("Name 2")] public string Name2 { get; set; }

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

    public ResourceResponse(DataResource30Service.Resource resource)
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
    }
}