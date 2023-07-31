using Blackbird.Applications.Sdk.Common;

namespace Blackbird.Plugins.Plunet.Models.Contacts;

public class CreateContactResponse
{
    [Display("Contact ID")]
    public string ContactId { get; set; }
}