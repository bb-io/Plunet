using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Customer;

public class GetProjectManagerResponse
{
    [Display("Project manager ID", Description = "The resource ID of the project manager assigned to the customer")]
    public string? ProjectManagerId { get; set; }

    [Display("Project manager name")]
    public string? ProjectManagerName { get; set; }
}
