using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.Contacts
{
    public class GetContactByExternalRequest
    {
        [Display("External ID")]
        public string ExternalId { get; set; }
    }
}
