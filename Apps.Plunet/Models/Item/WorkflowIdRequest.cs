using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Item;

public class WorkflowIdRequest
{
    [Display("Workflow ID")]
    [DataSource(typeof(WorkflowIdDataHandler))]
    public string WorkflowId { get; set; }
}
