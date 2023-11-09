using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Contacts;

public class CreateContactResponse
{
    [Display("Contact ID")]
    public string ContactId { get; set; }
}