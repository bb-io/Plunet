using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Item
{
    public class SearchItemsRequest : SearchInputBase
    {

        [Display("Item status")]
        [StaticDataSource(typeof(ItemStatusDataHandler))]
        public string? Status { get; set; }

        [Display("Document status")]
        [StaticDataSource(typeof(DocumentStatusDataHandler))]
        public string? DocumentStatus { get; set; }

        [Display("Only return IDs", Description = "If enabled, returns only IDs without fetching details for each item.")]
        public bool? OnlyReturnIds { get; set; }
    }
}
