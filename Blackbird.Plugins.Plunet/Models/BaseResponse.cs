using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models;

public class BaseResponse
{
    [Display("Status code")]
    public int StatusCode { get; set; }
}