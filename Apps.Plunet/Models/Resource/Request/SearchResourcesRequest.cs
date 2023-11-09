using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Apps.Plunet.DataSourceHandlers;

namespace Apps.Plunet.Models.Resource.Request
{
    public class SearchResourcesRequest
    {
        [Display("Contact ID")]
        public string? ContactId { get; set; }

        [Display("Resource type")]
        [DataSource(typeof(CustomerTypeDataHandler))]
        public string? ResourceType { get; set; }

        [Display("Email")]
        public string? Email { get; set; }

        [Display("Name 1")]
        public string? Name1 { get; set; }

        [Display("Name 2")]
        public string? Name2 { get; set; }

        [Display("Status")]
        [DataSource(typeof(ResourceStatusDataHandler))]
        public string? Status { get; set; }

        [Display("Source language")]
        [DataSource(typeof(LanguageIsoDataHandler))]
        public string? SourceLanguageCode { get; set; }

        [Display("Target language")]
        [DataSource(typeof(LanguageIsoDataHandler))]
        public string? TargetLanguageCode { get; set; }

        [Display("Workingi status")]
        [DataSource(typeof(WorkingStatusDataHandler))]
        public string? WorkingStatus { get; set; }

    }
}
