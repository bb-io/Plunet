using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Plunet.Actions;
using Apps.Plunet.Models.Request.Request;
using Tests.Plunet.Base;

namespace Tests.Plunet
{
    [TestClass]
    public class RequestTests:TestBase
    {
        [TestMethod]
        public async Task GetRequest_ReturnValue()
        { 
            var action= new RequestActions(InvocationContext);
            var result = await action.GetRequest("2");
            Console.WriteLine(result.CustomerId);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task CreateRequest_ReturnValue()
        {
            var action = new RequestActions(InvocationContext);
            var input = new CreatеRequestRequest { };
            var result = await action.CreateRequest(input);
            Console.WriteLine(result.RequestId);
            Assert.IsNotNull(result);
        }
    }
}
