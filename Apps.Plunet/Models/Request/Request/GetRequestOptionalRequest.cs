using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Request.Request;

public class GetRequestOptionalRequest
{
    [Display("Request ID")]
    public string? RequestId { get; set; }
}