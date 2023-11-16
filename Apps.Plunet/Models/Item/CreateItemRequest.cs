using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Item;

public class CreateItemRequest
{
    [Display("Project ID")]
    public string ProjectId { get; set; }
    
    [Display("Project type")]
    public int ProjectType { get; set; }
    
    [Display("Item name")]
    public string? ItemName { get; set; }

    [Display("Deadline date and time")]
    public DateTime? DeadlineDateTime { get; set; }

    [Display("Total price")]
    public double? TotalPrice { get; set; }
    
    public string? Reference { get; set; }
    
    public int? Status { get; set; }
}