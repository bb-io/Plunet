using Apps.Plunet.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Plunet.Base;

namespace Tests.Plunet
{
    [TestClass]
    public class PayableTests :TestBase
    {
        [TestMethod]
        public async Task UpdatePayables_returns_values()
        {
            var action = new PayableActions(InvocationContext, FileManager);
            var result = await action.UpdatePayable(new Apps.Plunet.Models.Payable.Request.UpdatePayableRequest { PayableId= "99471", Status="1"});

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(result);
        }
    }
}
