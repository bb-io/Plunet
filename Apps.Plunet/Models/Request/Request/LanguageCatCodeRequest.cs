using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;


namespace Apps.Plunet.Models.Request.Request
{
    public class LanguageCatCodeRequest
    {
        [Display("Language name")]
        [DataSource(typeof(LanguageNameDataHandler))]
        public string LanguageName { get; set; }

        [Display("CAT type")]
        [StaticDataSource(typeof(CatTypeDataHandler))]
        public string CatType { get; set; }
    }
}
