﻿using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class OrderResponse
{
    [Display("Currency")]
    public string Currency { get; set; }

    [Display("Customer ID")]
    public string CustomerId { get; set; }

    [Display("Delivery deadline")]
    public DateTime DeliveryDeadline { get; set; }

    [Display("Order closing date")]
    public DateTime OrderClosingDate { get; set; }

    [Display("Order date")]
    public DateTime OrderDate { get; set; }

    [Display("Order name")]
    public string OrderName { get; set; }

    [Display("Order ID")]
    public string OrderId { get; set; }

    [Display("Project manager ID")]
    public string ProjectManagerId { get; set; }

    [Display("Project name")]
    public string ProjectName { get; set; }

    [Display("Rate")]
    public double Rate { get; set; }

    public OrderResponse(DataOrder30Service.Order order)
    {
        Currency = order.currency;
        CustomerId = order.customerID.ToString();
        DeliveryDeadline = order.deliveryDeadline;
        OrderClosingDate = order.orderClosingDate;
        OrderDate = order.orderDate;
        OrderId = order.orderID.ToString();
        OrderName = order.orderDisplayName;
        ProjectManagerId = order.projectManagerID.ToString();
        ProjectName = order.projectName;
        Rate = order.rate;
    }
}