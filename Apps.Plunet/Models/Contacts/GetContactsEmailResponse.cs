using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.Contacts
{
    public class GetContactsEmailResponse
    {
        [Display("Email Addresses")]
        public IEnumerable<string> EmailAddresses { get; set; }
    }
}
