using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.ProjectCategory.Request;

public class CreateProjectCategoryRequest
{
    [Display("Project category")]
    public string ProjectCategory { get; set; }

    [Display("System language code")]
    public string? SystemLanguageCode { get; set; }
}