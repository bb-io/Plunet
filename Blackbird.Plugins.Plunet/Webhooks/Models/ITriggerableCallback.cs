using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackbird.Plugins.Plunet.Webhooks.Models
{
    public interface ITriggerableCallback
    {
        public TriggerContent GetTriggerContent();
    }
}
