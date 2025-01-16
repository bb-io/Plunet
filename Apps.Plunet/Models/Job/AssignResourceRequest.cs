using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Job
{
    public class AssignResourceRequest
    {
        [Display("Resource ID")]
        [DataSource(typeof(ResourceIdDataHandler))]
        public string ResourceId { get; set; }

        [Display("Round ID")]
        public string RoundId { get; set; }

        [Display("Resource contact ID")]
        public string? ResourceContactId { get; set; }
    }
}
