using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Models.Resource.Request
{
    public class CreateResourceRequest
    {
        [Display("Working status")]
        [StaticDataSource(typeof(WorkingStatusDataHandler))]
        public string WorkingStatus { get; set; }
    }
}
