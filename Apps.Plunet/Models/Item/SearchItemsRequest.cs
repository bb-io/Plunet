using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Item
{
    public class SearchItemsRequest
    {

        [Display("Item status")]
        [DataSource(typeof(ItemStatusDataHandler))]
        public string? Status { get; set; }

        [Display("Document status")]
        [DataSource(typeof(DocumentStatusDataHandler))]
        public string? DocumentStatus { get; set; }
    }
}
