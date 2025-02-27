using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Order;

public class CreateOrderByTemplateRequest
{
    [Display("Project manager ID")]
    [DataSource(typeof(ProjectManagerIdDataHandler))]
    public string? ProjectManagerId { get; set; }
    
    [Display("Customer ID")]
    [DataSource(typeof(CustomerIdDataHandler))]
    public string CustomerId { get; set; }

    [Display("Project name")]
    public string? ProjectName { get; set; }
    
    public string? Subject { get; set; }

    public DateTime? Deadline { get; set; }

    [Display("Contact ID")]
    [DataSource(typeof(ContactIdDataHandler))]
    public string? ContactId { get; set; }

    [DataSource(typeof(CurrencyDataSourceHandler))]
    public string? Currency { get; set; }
    
    public double? Rate { get; set; }
    
    [Display("Project manager memo")]
    public string? ProjectManagerMemo { get; set; }
    
    [Display("Reference number")]
    public string? ReferenceNumber { get; set; }

    //[Display("Status")]
    //[StaticDataSource(typeof(OrderStatusDataHandler))]
    //public string? Status { get; set; }

    [Display("Project category")]
    //[DataSource(typeof(ProjectCategoryDataHandler))]
    public string? ProjectCategory { get; set; }
}