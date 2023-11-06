using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Plugins.Plunet.DataSourceHandlers;

namespace Blackbird.Plugins.Plunet.Models;

public class LanguagesRequest
{
    [Display("Source language")]
    [DataSource(typeof(LanguageIsoDataHandler))]
    public string SourceLanguageCode { get; set; }

    [Display("Target language")]
    [DataSource(typeof(LanguageIsoDataHandler))]
    public string TargetLanguageCode { get; set; }
}