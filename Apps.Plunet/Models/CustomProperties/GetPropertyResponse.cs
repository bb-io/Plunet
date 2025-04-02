
using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.CustomProperties
{
    public class GetPropertyResponse
    {
        [Display("Property ID")]
        public int Id { get; set; }
        public string? Value { get; set; }
    }
}
