using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Contacts;

public class ContactRequest
{
    [Display("Contact")]
    [DataSource(typeof(ContactIdDataHandler))]
    public string ContactId { get; set; }
}