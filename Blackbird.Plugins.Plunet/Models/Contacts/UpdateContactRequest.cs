using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Plugins.Plunet.DataSourceHandlers;

namespace Blackbird.Plugins.Plunet.Models.Contacts;

public class UpdateContactRequest : CreateContactRequest
{
    [Display("Contact ID")]
    [DataSource(typeof(ContactIdDataHandler))]
    public string ContactId { get; set; }
}