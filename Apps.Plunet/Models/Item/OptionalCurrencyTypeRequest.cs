using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Item
{
    public class OptionalCurrencyTypeRequest
    {
        [Display("Currency type")]
        [DataSource(typeof(CurrencyTypeDataHandler))]
        public string? CurrencyType { get; set; }
    }
}
