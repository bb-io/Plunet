using Blackbird.Plugins.Plunet.DataOrder30Service;

namespace Apps.Plunet.Extensions;

public static class OrderExtensions
{
    public static string ToDisplayName(this Order order)
    {
        return string.IsNullOrWhiteSpace(order.projectName) ? $"Unnamed order (ID: {order.orderID})" : order.projectName;
    }
}