using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Item
{
    public class OptionalItemProjectRequest
    {

        [Display("Type")]
        [DataSource(typeof(ItemProjectTypeDataHandler))]
        public string ProjectType { get; set; }

        [Display("Order or quote ID")]
        [DataSource(typeof(ProjectDataSourceHandler))]
        public string? ProjectId { get; set; }
    }
}
