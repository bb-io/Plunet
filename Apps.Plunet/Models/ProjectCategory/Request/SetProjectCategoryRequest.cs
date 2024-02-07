using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.ProjectCategory.Request;

public class SetProjectCategoryRequest
{
    [Display("Project category")]
    [DataSource(typeof(ProjectCategoryDataHandler))]
    public string ProjectCategory { get; set; }

    [Display("System language code")]
    public string? SystemLanguageCode { get; set; }
}