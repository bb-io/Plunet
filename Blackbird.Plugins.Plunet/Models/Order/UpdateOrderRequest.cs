﻿using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class UpdateOrderRequest : CreateOrderRequest
{
    [Display("Order ID")] public string OrderId { get; set; }
}