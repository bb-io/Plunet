using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Models.Item;

public class CurrencyTypeRequest
{
    [Display("Currency type"), StaticDataSource(typeof(CurrencyTypeDataHandler))]
    public string? CurrencyType { get; set; }
}
