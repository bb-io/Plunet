using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Plugins.Plunet.DataSourceHandlers;

namespace Blackbird.Plugins.Plunet.Models.Quote;

public class CreateQuoteRequest
{
    [Display("Currency")]
    public string? Currency { get; set; }

    [Display("Customer")]
    [DataSource(typeof(CustomerIdDataHandler))]
    public string? CustomerId { get; set; }

    [Display("Project manager Memo")]
    public string? ProjectManagerMemo { get; set; }

    [Display("Project name")]
    public string? ProjectName { get; set; }

    [Display("Reference number")]
    public string? ReferenceNumber { get; set; }

    [Display("Subject")]
    public string? Subject { get; set; }

    [Display("Status")]
    public int? Status { get; set; }

}