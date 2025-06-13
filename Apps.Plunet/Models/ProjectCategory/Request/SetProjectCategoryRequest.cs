using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.ProjectCategory.Request;

public class SetProjectCategoryRequest
{
    [Display("Project category")]
    //[StaticDataSource(typeof(ProjectCategoryDataHandler))]
    public string ProjectCategory { get; set; }
}