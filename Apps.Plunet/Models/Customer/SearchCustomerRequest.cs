using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Customer
{
    public class SearchCustomerRequest
    {
        [Display("Customer type")]
        [StaticDataSource(typeof(CustomerTypeDataHandler))]
        public string? CustomerType { get; set; }

        [Display("Email")]
        public string? Email { get; set; }

        [Display("Source language")]
        [DataSource(typeof(LanguageIsoDataHandler))]
        public string? SourceLanguageCode { get; set; }

        [Display("Name 1")]
        public string? Name1 { get; set; }

        [Display("Name 2")]
        public string? Name2 { get; set; }

        [Display("Status")]
        [StaticDataSource(typeof(CustomerStatusDataHandler))]
        public string? Status { get; set; }
    }
}
