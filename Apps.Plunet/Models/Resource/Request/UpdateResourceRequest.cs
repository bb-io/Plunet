using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Resource.Request;

public class UpdateResourceRequest : ResourceRequest
{
    [Display("Academic title")] 
    public string? AcademicTitle { get; set; }
    
    [Display("Cost center")]
    public string? CostCenter { get; set; }
    
    [Display("Currency"), DataSource(typeof(CurrencyDataSourceHandler))] 
    public string? Currency { get; set; }
    
    [Display("Email")] 
    public string? Email { get; set; }
    
    [Display("External ID")] 
    public string? ExternalId { get; set; }
    
    [Display("Fax")] 
    public string? Fax { get; set; }
    
    [Display("Form of address")] 
    public string? FormOfAddress { get; set; }
    
    [Display("Full name")] 
    public string? FullName { get; set; }
    
    [Display("Mobile phone")] 
    public string? MobilePhone { get; set; }
    
    [Display("Name 1")] 
    public string? Name1 { get; set; }
    
    [Display("Name 2")] 
    public string? Name2 { get; set; }
    
    [Display("Opening")] 
    public string? Opening { get; set; }
    
    [Display("Phone")] 
    public string? Phone { get; set; }
    
    [Display("Skype")]
    public string? SkypeId { get; set; }
    
    [Display("Website")] 
    public string? Website { get; set; }
    
    [Display("Status"), StaticDataSource(typeof(ResourceStatusDataHandler))] 
    public string? Status { get; set; }
    
    [Display("Supervisor 1")] 
    public string? Supervisor1 { get; set; }
    
    [Display("Supervisor 2")] 
    public string? Supervisor2 { get; set; }
    
    [Display("User ID")] 
    public string? UserId { get; set; }
    
    [Display("Working status"), StaticDataSource(typeof(WorkingStatusDataHandler))] 
    public string? WorkingStatus { get; set; }
}