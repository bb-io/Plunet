using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Webhooks.Models.Parameters;

public class CustomerEntryTypeOptionalRequest
{
    [Display("Customer entry type"), StaticDataSource(typeof(CustomerEntryTypeDataHandler))]
    public string? CustomerEntryType { get; set; }
}