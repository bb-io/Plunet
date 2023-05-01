using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackbird.Plugins.Plunet.Webhooks.Utils
{
    public enum EventType
    {
        STATUS_CHANGED = 1,
        NEW_ENTRY_CREATED = 2,
        ENTRY_DELETED = 3,
        START_DATE_CHANGED = 4,
        DELIVERY_DATE_CHANGED = 5
    }
}
