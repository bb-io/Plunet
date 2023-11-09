
using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Request.Request;

public class UpdateRequestRequest : CreatеRequestRequest
{
    [Display("Request ID")]
    public string RequestId { get; set; }

}