using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.CustomProperties;

public class FindJobByTextModule : FindByTextModuleRequest
{
    [Display("Entity type")]
    [StaticDataSource(typeof (TextModuleUsageDataHandler))]
    public string UsageArea {  get; set; } 
}