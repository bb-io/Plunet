using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.ProjectCategory.Request;

public class GetProjectCategoryRequest
{
    [Display("System language code")]
    public string? SystemLanguageCode { get; set; }
}