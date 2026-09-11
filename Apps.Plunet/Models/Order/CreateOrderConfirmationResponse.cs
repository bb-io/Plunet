using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Order;

public record CreateOrderConfirmationResponse
{
    [Display("Order confirmation file location")]
    public string? OrderConfirmationFileLocation { get; set; }
}