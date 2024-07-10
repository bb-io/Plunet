using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.Webhooks.Polling
{
    public class PayableCreatedInput
    {
        [StaticDataSource(typeof(PayableStatusDataHandler))]
        public string? Status { get; set; }
    }
}
