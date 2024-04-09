using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Item
{
    public class OptionalJobStatusRequest
    {
        [Display("Status")]
        [StaticDataSource(typeof(JobStatusDataHandler))]
        public string? Status { get; set; }
    }
}
