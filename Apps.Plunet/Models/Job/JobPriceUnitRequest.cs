using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Job
{
    public class JobPriceUnitRequest
    {
        [Display("Service")]
        [DataSource(typeof(ServiceNameDataHandler))]
        public string Service { get; set; }

        [Display("Price unit")]
        [DataSource(typeof(JobPriceUnitDataHandler))]
        public string PriceUnit { get; set; }
    }
}
