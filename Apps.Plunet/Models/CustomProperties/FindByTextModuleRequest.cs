using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.CustomProperties;

public abstract class FindByTextModuleRequest
{
    [Display("Flag", Description = "e.g. [Textmodule_1]")]
    public string Flag { get; set; }
    
    [Display("Text module value")]
    public string TextModuleValue { get; set; }
}