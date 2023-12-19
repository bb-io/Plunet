using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Item
{
    public class OptionalJobStatusRequest
    {
        [Display("Status")]
        [DataSource(typeof(JobStatusDataHandler))]
        public string? Status { get; set; }
    }
}
