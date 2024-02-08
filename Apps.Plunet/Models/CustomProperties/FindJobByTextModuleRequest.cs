using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.CustomProperties;

public class FindJobByTextModuleRequest
{
    [Display("Entity type")]
    [DataSource(typeof (TextModuleUsageDataHandler))]
    public string UsageArea {  get; set; }

    [Display("Flag", Description = "e.g. [Textmodule_1]")]
    public string Flag { get; set; }
    
    [Display("Text module value")]
    public string TextModuleValue { get; set; }
}