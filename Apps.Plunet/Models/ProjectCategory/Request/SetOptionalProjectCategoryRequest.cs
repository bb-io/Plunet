using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.ProjectCategory.Request
{
    public class SetOptionalProjectCategoryRequest
    {
        [Display("Project category")]
        [DataSource(typeof(ProjectCategoryDataHandler))]
        public string? ProjectCategory { get; set; }
    }
}
