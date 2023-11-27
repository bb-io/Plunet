using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataItem30Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Actions
{
    [ActionList]
    public class ItemActions : PlunetInvocable
    {
        public ItemActions(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        
        public async Task<Item[]> GetItems(int orderId)
        {
            
            // var order = await OrderClient.getOrderObjectAsync(Uuid, orderId);
            //var id = await OrderClient.getMasterProjectIDAsync(Uuid, orderId);
            var items = await ItemClient.getItemsByStatus2Async(Uuid, orderId, 3, 8);
            return items.data;
        }
    }
}
