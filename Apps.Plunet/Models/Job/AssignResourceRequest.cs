using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Job
{
    public class AssignResourceRequest
    {
        [Display("Resource ID")]
        [DataSource(typeof(ResourceIdDataHandler))]
        public string ResourceId { get; set; }

        [Display("Round ID")]
        [DataSource(typeof(RoundDataHandler))]
        public string RoundId { get; set; }

        [Display("Resource contact ID")]
        public string? ResourceContactId { get; set; }

        [Display("Type")]
        [StaticDataSource(typeof(ItemProjectTypeDataHandler))]
        public string ProjectType { get; set; }

        [Display("Job ID")]
        public string JobId { get; set; }
    }
}
