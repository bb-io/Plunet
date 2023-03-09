namespace Blackbird.Plugins.Plunet.Models.Order;

public class CreateOrderRequest
{    
    public int CustomerId { get; set; }

    public string ProjectName { get; set; }

    public DateTime Deadline { get; set; }
}