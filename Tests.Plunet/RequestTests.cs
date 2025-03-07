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
            var result = await action.GetRequest("8");
            Console.WriteLine($"Customer Id: " + result.CustomerId);
            Console.WriteLine($"Request number: " + result.RequestNumber);
            Console.WriteLine($"Customer contact id: " + result.CustomerContactId);
            Console.WriteLine($"Customer ref: "+result.CustomerRefNo);
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

        [TestMethod]
        public async Task SearchRequest_ReturnValue()
        {
            var action = new RequestActions(InvocationContext);
            var input = new SearchRequestsInput { };
            var result = await action.SearchRequests(input);

            foreach (var item in result.Items)
            {
                Console.WriteLine($"Request Id: " + item.RequestId);
                Console.WriteLine($"Request number: " + item.RequestNumber);
                Console.WriteLine($"Customer contact id: " + item.CustomerContactId);
                Console.WriteLine($"Customer ref: " + item.CustomerRefNo);
                Assert.IsNotNull(item);
            }
        }
    }
}
