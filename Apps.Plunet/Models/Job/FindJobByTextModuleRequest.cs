using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Job;

public class FindJobByTextModuleRequest
{
    public List<string> Jobs { get; set; }
    [Display("Type")]
    [DataSource(typeof(ItemProjectTypeDataHandler))]
    public string ProjectType { get; set; }
}