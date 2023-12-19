using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Contacts
{
    public class GetContactByExternalRequest
    {
        [Display("External ID")]
        public string ExternalId { get; set; }
    }
}
