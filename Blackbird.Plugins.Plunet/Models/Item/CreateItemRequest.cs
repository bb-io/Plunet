namespace Blackbird.Plugins.Plunet.Models.Item;

public class CreateItemRequest
{
   // public string UUID { get; set; }

    public int OrderId { get; set; }

    public string ItemName { get; set; }

    public DateTime DeadlineDateTime { get; set; }

    public double TotalPrice { get; set; }
}