using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models;

public class AddLanguageCombinationRequest
{
    [Display("Source language")]
    [DataSource(typeof(LanguageIsoDataHandler))]
    public string SourceLanguageCode { get; set; }

    [Display("Target language")]
    [DataSource(typeof(LanguageIsoDataHandler))]
    public string TargetLanguageCode { get; set; }
}