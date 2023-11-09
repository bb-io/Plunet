using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Order;

public class CreateOrderRequest
{
    [Display("Project manager ID")]
    [DataSource(typeof(ProjectManagerIdDataHandler))]
    public string ProjectManagerId { get; set; }
    
    [Display("Customer")]
    [DataSource(typeof(CustomerIdDataHandler))]
    public string? CustomerId { get; set; }

    [Display("Project name")]
    public string? ProjectName { get; set; }
    
    public string? Subject { get; set; }

    public DateTime? Deadline { get; set; }

    [Display("Contact ID")]
    [DataSource(typeof(ContactIdDataHandler))]
    public string? ContactId { get; set; }
    
    public string? Currency { get; set; }
    
    public double? Rate { get; set; }
    
    [Display("Project manager memo")]
    public string? ProjectManagerMemo { get; set; }
    
    [Display("Reference number")]
    public string? ReferenceNumber { get; set; }
}