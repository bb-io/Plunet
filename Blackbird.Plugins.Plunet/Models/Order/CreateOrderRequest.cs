﻿using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class CreateOrderRequest
{
    [Display("Project manager ID")]
    public string ProjectManagerId { get; set; }
    
    [Display("Customer ID")]
    public string? CustomerId { get; set; }

    [Display("Project name")]
    public string? ProjectName { get; set; }
    
    public string? Subject { get; set; }

    public DateTime? Deadline { get; set; }

    [Display("Contact ID")]
    public string? ContactId { get; set; }
    
    public string? Currency { get; set; }
    
    public double? Rate { get; set; }
    
    [Display("Project manager memo")]
    public string? ProjectManagerMemo { get; set; }
    
    [Display("Reference number")]
    public string? ReferenceNumber { get; set; }
}