using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models
{
    public class OptionalTargetLanguageRequest
    {
        [Display("Target language")]
        [DataSource(typeof(LanguageNameDataHandler))]
        public string? TargetLanguageName { get; set; }
    }
}
