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
            var input = new SearchRequestsInput { Limit=10, OnlyReturnIds=true };
            var result = await action.SearchRequests(input);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            Console.WriteLine(json);

            Assert.IsNotNull(result);
        }
    }
}
