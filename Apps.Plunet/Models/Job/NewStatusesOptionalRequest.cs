using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Models.Job
{
    public class NewStatusesOptionalRequest
    {
        [Display("New status")]
        [StaticDataSource(typeof(JobStatusDataHandler))]
        public IEnumerable<string>? Statuses { get; set; }
    }
}
