namespace Blackbird.Plugins.Plunet.Models.Order;

public class OrderResponse
{
    public string Currency { get; set; }

    public int CustomerId { get; set; }

    public DateTime DeliveryDeadline { get; set; }

    public DateTime OrderClosingDate { get; set; }

    public DateTime OrderDate { get; set; }

    public string OrderName { get; set; }

    public int OrderId { get; set; }

    public int ProjectManagerId { get; set; }

    public string ProjectName { get; set; }

    public double Rate { get; set; }
}