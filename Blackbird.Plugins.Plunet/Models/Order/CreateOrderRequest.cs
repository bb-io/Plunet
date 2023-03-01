namespace Blackbird.Plugins.Plunet.Models.Order;

public class CreateOrderRequest
{
    public string UUID { get; set; }
    
    public int CustomerId { get; set; }

    public string ProjectName { get; set; }

    public DateTime Deadline { get; set; }
}