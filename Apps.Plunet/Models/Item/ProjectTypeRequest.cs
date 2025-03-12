using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Models.Item;

public class ProjectTypeRequest
{
    [Display("Project type"), StaticDataSource(typeof(ItemProjectTypeDataHandler))]
    public string ProjectType { get; set; } = string.Empty;
}
